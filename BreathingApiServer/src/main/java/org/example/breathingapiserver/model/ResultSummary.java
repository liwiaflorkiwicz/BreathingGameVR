package org.example.breathingapiserver.model;

public class ResultSummary {
    private float overallAccuracy;
    private String summaryTable;
    private String sessionId;

    public float getOverallAccuracy() { return overallAccuracy; }
    public void setOverallAccuracy(float overallAccuracy) { this.overallAccuracy = overallAccuracy; }
    public String getSummaryTable() { return summaryTable; }
    public void setSummaryTable(String summaryTable) { this.summaryTable = summaryTable; }
    public String getSessionId() { return sessionId; }
    public void setSessionId(String sessionId) { this.sessionId = sessionId; }
}
