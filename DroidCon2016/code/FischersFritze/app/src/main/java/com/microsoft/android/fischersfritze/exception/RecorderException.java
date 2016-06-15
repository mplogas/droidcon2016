package com.microsoft.android.fischersfritze.exception;

/**
 * Created by mplogas on 13.06.16.
 */
public class RecorderException extends Exception {
    public RecorderException(String message) {super(message);}
    public RecorderException(String message, Exception e) {super(message, e);}
}
