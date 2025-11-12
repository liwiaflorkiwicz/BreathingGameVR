package org.example.breathingapiserver.model;

import java.util.List;

public class GameSessionData {
    private float inhale;
    private float exhale;
    private int reps;
    private List<ControllerFrame> controllersData;

    public float getInhale() { return inhale; }
    public void setInhale(float inhale) { this.inhale = inhale; }
    public float getExhale() { return exhale; }
    public void setExhale(float exhale) { this.exhale = exhale; }
    public int getReps() { return reps; }
    public void setReps(int reps) { this.reps = reps; }
    public List<ControllerFrame> getControllersData() { return controllersData; }
    public void setControllersData(List<ControllerFrame> controllersData) { this.controllersData = controllersData; }
}
