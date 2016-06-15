package com.microsoft.android.fischersfritze.recorder;

import android.media.MediaPlayer;
import android.media.MediaRecorder;
import android.os.Environment;
import android.os.Parcel;
import android.os.Parcelable;
import android.util.Log;
import com.microsoft.android.fischersfritze.exception.RecorderException;

import java.io.IOException;

/**
 * Created by mplogas on 13.06.16.
 */
public class Recorder {
    private static final String LOG_TAG = "Recorder";

    private MediaRecorder recorder = null;
    private MediaPlayer player = null;

    private String fileName;
    private boolean isRecording = false;
    private boolean isPlaying = false;


    public void record() throws RecorderException {
        if(recorder == null || player == null) throw new RecorderException("Class not initialized yet. Call start() first!");

        if (!isRecording && !isPlaying) {
            startRecording();
        } else {
            stopRecording();
        }
        isRecording = !isRecording;
    }

    public void play() throws RecorderException {
        if(recorder == null || player == null) throw new RecorderException("Class not initialized yet. Call start() first!");

        if (!isPlaying && !isRecording) {
            startPlaying();
        } else {
            stopPlaying();
        }
        isPlaying = !isPlaying;
    }

    public void suspend() {
        if (recorder != null) {
            recorder.release();
            recorder = null;
            isRecording = false;
        }

        if (player != null) {
            player.release();
            player = null;
            isPlaying = false;
        }
    }

    public void start(MediaRecorder recorder, MediaPlayer player) {
        if(recorder == null) throw new IllegalArgumentException("recorder MUST NOT be NULL");
        if(player == null) throw new IllegalArgumentException("player MUST NOT be NULL");

        this.recorder = recorder;
        this.player = player;
        this.fileName = buildFilePath();
    }

    public boolean isRecording() {return this.isRecording;}
    public boolean isPlaying() {return this.isPlaying;}
    public boolean isInitialzed() {return (recorder == null && player == null);}

    private String buildFilePath() {
        String result = Environment.getExternalStorageDirectory().getAbsolutePath();
        result += "/fischersfritze.3gp";

        return result;
    }

    private void startPlaying() {
        try {
            player.setDataSource(fileName);
            player.prepare();
            player.start();
        } catch (IOException e) {
            Log.e(LOG_TAG, "prepare() failed");
        }
    }

    private void stopPlaying() {
        player.release();
        //player = null;
    }

    private void startRecording() {
        recorder.setAudioSource(MediaRecorder.AudioSource.MIC);
        recorder.setOutputFormat(MediaRecorder.OutputFormat.THREE_GPP);
        recorder.setOutputFile(fileName);
        recorder.setAudioEncoder(MediaRecorder.AudioEncoder.AMR_NB);

        try {
            recorder.prepare();
        } catch (IOException e) {
            Log.e(LOG_TAG, "prepare() failed");
        }

        recorder.start();
    }

    private void stopRecording() {
        recorder.stop();
        recorder.release();
        //recorder = null;
    }
}
