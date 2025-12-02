package org.example.breathingapiserver.model;

import java.util.List;

public class ResultSummary {
    private float overallAccuracy;
    private String summaryTable;
    private String sessionId;

    private List<ControllerFrame> graphData;

    public float getOverallAccuracy() { return overallAccuracy; }
    public void setOverallAccuracy(float overallAccuracy) { this.overallAccuracy = overallAccuracy; }
    public String getSummaryTable() { return summaryTable; }
    public void setSummaryTable(String summaryTable) { this.summaryTable = summaryTable; }
    public String getSessionId() { return sessionId; }
    public void setSessionId(String sessionId) { this.sessionId = sessionId; }

    public List<ControllerFrame> getGraphData() { return graphData; }
    public void setGraphData(List<ControllerFrame> graphData) { this.graphData = graphData; }

}
