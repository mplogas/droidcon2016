package com.microsoft.android.fischersfritze;

import android.Manifest;
import android.app.Fragment;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.TextView;
import com.microsoft.android.fischersfritze.oxford.Helper;
import com.microsoft.projectoxford.speechrecognition.*;
import com.microsoft.bing.speech.SpeechClientStatus;
import pub.devrel.easypermissions.EasyPermissions;

import java.util.List;

public class GameFragment extends Fragment implements ISpeechRecognitionServerEvents, EasyPermissions.PermissionCallbacks {

    private static final String LOG_TAG = "GameFragment";
    private static final String[] PERMISSIONS = {Manifest.permission.RECORD_AUDIO, Manifest.permission.WRITE_EXTERNAL_STORAGE, Manifest.permission.READ_EXTERNAL_STORAGE};
    private static final int PERMISSIONS_REQ = 1;

    //ui
    private TextView tonguetwisterText;
    private EditText resultText;
    private TextView scoreText;
    private FloatingActionButton recordButton = null;
    private FloatingActionButton newButton = null;

    //recording
    private boolean isRecording;

    //prefs
    private SharedPreferences sharedPreferences;
    private String displayName;
    private SpeechRecognitionMode recDuration = SpeechRecognitionMode.ShortPhrase;
    private String language;

    private SharedPreferences.OnSharedPreferenceChangeListener sharedPrefslistener =
            new SharedPreferences.OnSharedPreferenceChangeListener() {
                public void onSharedPreferenceChanged(SharedPreferences prefs, String key) {
                    if(key.equals(getString(R.string.pref_display_name_key))) {
                        setDisplayName(prefs.getString(getString(R.string.pref_display_name_key), getString(R.string.pref_display_name_default)));
                        writeLineToResult(getString(R.string.pref_display_name_key) + " changed to " + prefs.getString(getString(R.string.pref_display_name_key), getString(R.string.pref_display_name_default)));
                    } else if (key.equals(getString(R.string.pref_rec_duration_key))) {
                        setRecordingDuration(prefs.getString(getString(R.string.pref_rec_duration_key), getString(R.string.pref_rec_duration_default)));
                        writeLineToResult(getString(R.string.pref_rec_duration_key) + " changed to " + prefs.getString(getString(R.string.pref_rec_duration_key), getString(R.string.pref_rec_duration_default)));
                    } else if (key.equals(getString(R.string.pref_language_key))) {
                        setLanguage(prefs.getString(getString(R.string.pref_language_key), getString(R.string.pref_language_default)));
                        writeLineToResult(getString(R.string.pref_language_key) + " changed to " + prefs.getString(getString(R.string.pref_language_key), getString(R.string.pref_language_default)));
                    }

                    shutdownMicrophoneClient();
                    initializeMicrophoneClient();
                }
            };

    //oxford
    private MicrophoneRecognitionClient micClient = null;
    private FinalResponseStatus isReceivedResponse = FinalResponseStatus.NotReceived;

    //scoring
    private String currentTongueTwister;

    public GameFragment() {
        // Required empty public constructor
    }

    public static GameFragment newInstance() {
        return new GameFragment();
    }

    //lifecycle
    @Override
    public void onAttach(Context context) {
        super.onAttach(context);
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_game, container, false);

        requestPermissions();
        recordButton = (FloatingActionButton) view.findViewById(R.id.FAB_record);
        newButton = (FloatingActionButton) view.findViewById(R.id.FAB_new);
        tonguetwisterText = (TextView) view.findViewById(R.id.textView_tonguetwister);
        resultText = (EditText) view.findViewById(R.id.editText_result);
        scoreText = (TextView) view.findViewById(R.id.textView_score);
        setUpFloatingActionButtons();

        sharedPreferences = PreferenceManager.getDefaultSharedPreferences(view.getContext());
        sharedPreferences.registerOnSharedPreferenceChangeListener(sharedPrefslistener);
        setDisplayName(sharedPreferences.getString(getString(R.string.pref_display_name_key), getString(R.string.pref_display_name_default)));
        setRecordingDuration(sharedPreferences.getString(getString(R.string.pref_rec_duration_key), getString(R.string.pref_rec_duration_default)));
        setLanguage(sharedPreferences.getString(getString(R.string.pref_language_key), getString(R.string.pref_language_default)));

        setUpTongueTwister();

        return view;
    }



    @Override
    public void onResume() {
        super.onResume();

        if (EasyPermissions.hasPermissions(getContext(), PERMISSIONS)) {
            initializeMicrophoneClient();
            writeLineToResult("--- client initialized ---");
            writeLineToResult();
        }
    }

    @Override
    public void onPause() {
        super.onPause();

        if (EasyPermissions.hasPermissions(getContext(), PERMISSIONS))
        {
            shutdownMicrophoneClient();
            writeLineToResult("--- client uninitialized ---");
            writeLineToResult();
        }
    }

    @Override
    public void onDestroyView() {
        sharedPreferences.unregisterOnSharedPreferenceChangeListener(sharedPrefslistener);

        super.onDestroyView();
    }

    @Override
    public void onDetach() {
        super.onDetach();
    }

    //ui
    private void setNotRecording() {
        recordButton.setImageResource(android.R.drawable.ic_btn_speak_now);
        Snackbar.make(getView(), "Not Recording", Snackbar.LENGTH_LONG).show();
        isRecording = false;
    }

    private void setRecording() {
        isRecording = true;
        recordButton.setImageResource(android.R.drawable.ic_menu_close_clear_cancel);
        Snackbar.make(getView(), "Recording...", Snackbar.LENGTH_LONG).show();
    }

    private void setUpFloatingActionButtons() {
        recordButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (EasyPermissions.hasPermissions(getContext(), PERMISSIONS)) {
                    if(!isRecording) {
                        setRecording();
                        startRecording();
                    } else {
                        setNotRecording();
                        //stopRecording();
                    }
                }
            }
        });
        newButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(!isRecording) {
                    clearAll();
                    setUpTongueTwister();
                }
            }
        });
    }

    private void writeLineToResult() {
        resultText.append("");
    }

    private void writeLineToResult(String s) {
        resultText.append(s + "\n");
    }

    private void setUpTongueTwister() {
        currentTongueTwister = Helper.getRandomTongueTwister(getLanguage());
        tonguetwisterText.setText(currentTongueTwister);
    }

    private void calculateResult(String result) {
        int distance = Helper.levenshteinDistance(currentTongueTwister, result);
        scoreText.setText(String.format(getString(R.string.score_text), 100 - distance));


    }

    private void clearAll() {
        currentTongueTwister = "";
        tonguetwisterText.setText("");
        resultText.setText("");
        scoreText.setText("");
    }

    // prefs
    private String getDisplayName() {
        return this.displayName;
    }

    private void setDisplayName(String name) {
        this.displayName = name;
    }

    private SpeechRecognitionMode getRecordingDuration () {
        return recDuration;
    }

    private void setRecordingDuration(String magicNumber) {
        if(magicNumber.equals("1")) recDuration = SpeechRecognitionMode.LongDictation;
        else recDuration = SpeechRecognitionMode.ShortPhrase;
    }

    private String getLanguage() {
        return language;
    }
    private void setLanguage(String language) {
        this.language = language;
    }

    //permissions
    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        // Forward results to EasyPermissions
        EasyPermissions.onRequestPermissionsResult(requestCode, permissions, grantResults, this);
    }

    @Override
    public void onPermissionsGranted(int requestCode, List<String> perms) {
        Log.d(LOG_TAG, "onPermissionsGranted:" + requestCode + ":" + perms.size());
    }

    @Override
    public void onPermissionsDenied(int requestCode, List<String> perms) {
        Log.d(LOG_TAG, "onPermissionsDenied:" + requestCode + ":" + perms.size());
    }

    private void requestPermissions() {
        if(!EasyPermissions.hasPermissions(getContext(), PERMISSIONS)) {
            EasyPermissions.requestPermissions(this, getString(R.string.permissions), PERMISSIONS_REQ , PERMISSIONS);
        }
    }

   //oxford
    private enum FinalResponseStatus { NotReceived, OK, Timeout }
    private String getPrimaryKey() {
        return getString(R.string.primaryKey);
    }
    private String getSecondaryKey() {
        return getString(R.string.secondaryKey);
    }

    private void initializeMicrophoneClient() {
        try {
            if (this.micClient == null) {
                this.micClient = SpeechRecognitionServiceFactory.createMicrophoneClient(
                        getActivity(),
                        this.getRecordingDuration(),
                        this.getLanguage(),
                        this,
                        this.getPrimaryKey(),
                        this.getSecondaryKey());
            }
        } catch (Exception e) {
            writeLineToResult("mic initialization exception: " + e.getMessage());
        }
    }

    private void shutdownMicrophoneClient() {
        if (this.micClient != null) {
            this.micClient.endMicAndRecognition();
            try {
                this.micClient.finalize();
            } catch (Throwable throwable) {
                throwable.printStackTrace();
            }
            this.micClient = null;
        }
    }

    private void startRecording() {
        if(micClient != null) {
            this.micClient.startMicAndRecognition();
        }

    }

    private void stopRecording() {
        if (this.micClient != null) {
            this.micClient.endMicAndRecognition();
        }
    }

    @Override
    public void onPartialResponseReceived(String response) {
        this.writeLineToResult("--- Partial result received by onPartialResponseReceived() ---");
        this.writeLineToResult(response);
        this.writeLineToResult();
    }

    @Override
    public void onFinalResponseReceived(RecognitionResult response) {
        boolean isFinalDictationMessage = this.getRecordingDuration() == SpeechRecognitionMode.LongDictation &&
                (response.RecognitionStatus == RecognitionStatus.EndOfDictation ||
                        response.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout);

        if (this.micClient != null && ((this.getRecordingDuration() == SpeechRecognitionMode.ShortPhrase) || isFinalDictationMessage)) {
            // we got the final result, so it we can end the mic reco.  No need to do this
            // for dataReco, since we already called endAudio() on it as soon as we were done
            // sending all the data.
            stopRecording();
        }

        if (isFinalDictationMessage) {
            setNotRecording();
            this.isReceivedResponse = FinalResponseStatus.OK;
        }

        if (!isFinalDictationMessage) {
            String bestMatch = "";
            int bestMatchScore = 100;

            this.writeLineToResult("********* Final n-BEST Results *********");
            if (response.Results.length > 0) {

                for (int i = 0; i < response.Results.length; i++) {
                    int calculatedScore = Helper.levenshteinDistance(currentTongueTwister, response.Results[i].DisplayText);

                    this.writeLineToResult("[" + i + "]" + " Confidence=" + response.Results[i].Confidence +
                            " Text=\"" + response.Results[i].DisplayText + "\"");
                    this.writeLineToResult("Calculated score: " + calculatedScore);

                    if(calculatedScore <= bestMatchScore) {
                        bestMatch = response.Results[i].DisplayText;
                        bestMatchScore = calculatedScore;
                    }

                    this.writeLineToResult();
                }

                calculateResult(response.Results[0].DisplayText);
                this.writeLineToResult("-------- Scoring --------");
                this.writeLineToResult("expected string: " + currentTongueTwister);
                this.writeLineToResult("best match: " + bestMatch);
                this.writeLineToResult("best score: " + bestMatchScore);
            }

            this.writeLineToResult();
        }
    }



    @Override
    public void onIntentReceived(String payload) {
        this.writeLineToResult("--- Intent received by onIntentReceived() ---");
        this.writeLineToResult(payload);
        this.writeLineToResult();
    }

    @Override
    public void onError(int errorCode, String response) {
        setNotRecording();
        this.writeLineToResult("--- Error received by onError() ---");
        this.writeLineToResult("Error code: " + SpeechClientStatus.fromInt(errorCode) + " " + errorCode);
        this.writeLineToResult("Error text: " + response);
        this.writeLineToResult();
    }

    @Override
    public void onAudioEvent(boolean recording) {
        this.writeLineToResult("--- Microphone status change received by onAudioEvent() ---");
        this.writeLineToResult("********* Microphone status: " + recording + " *********");
        if (recording) {
            this.writeLineToResult("Please start speaking.");
        }

        writeLineToResult();
        if (!recording) {
            this.micClient.endMicAndRecognition();
            setNotRecording();
        }
    }
}
