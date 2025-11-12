package org.example.breathingapiserver.controller;

import org.example.breathingapiserver.model.ControllerFrame;
import org.example.breathingapiserver.model.GameSessionData;
import org.example.breathingapiserver.model.PreSessionData;
import org.example.breathingapiserver.model.ResultSummary;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import jakarta.annotation.PostConstruct;
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/api/data")
public class DataController {

    private static final String SAVE_DIR = "sessions";
    private final ObjectMapper objectMapper = new ObjectMapper();
    private final Path savePath = Paths.get(SAVE_DIR);

    private final HashMap<String, PreSessionData> activeSessions = new HashMap<>();

    @PostConstruct
    public void init() {
        try {
            if (!Files.exists(savePath)) {
                Files.createDirectories(savePath);
                System.out.println("Created directory: " + savePath.toAbsolutePath());
            }
        } catch (IOException e) {
            System.err.println("Error while creating directory: " + e.getMessage());
        }
    }

    @PostMapping("/presession")
    public ResponseEntity<?> receivePreSessionData(@RequestBody PreSessionData preData) {
        if (preData == null) {
            return new ResponseEntity<>("No JSON data", HttpStatus.BAD_REQUEST);
        }

        // generate random session UUID
        String sessionId = UUID.randomUUID().toString();
        activeSessions.put(sessionId, preData);

        System.out.println("Captured data from presession with sessionId: " + sessionId);
        return new ResponseEntity<>(
                new ResponseStatus("success", "Pre-session data received", sessionId),
                HttpStatus.OK
        );
    }

    // Sending controllers data
    @PostMapping("/postsession/{sessionId}")
    public ResponseEntity<?> receivePostSessionData(@PathVariable String sessionId,
                                                    @RequestBody GameSessionData postData) {

        PreSessionData preData = activeSessions.get(sessionId);
        if (preData == null) {
            return new ResponseEntity<>("Couldn't find active session for sessionId: " + sessionId, HttpStatus.NOT_FOUND);
        }
        if (postData == null || postData.getControllersData() == null) {
            return new ResponseEntity<>("No controllers data", HttpStatus.BAD_REQUEST);
        }

        postData.setInhale(preData.getInhale());
        postData.setExhale(preData.getExhale());
        postData.setReps(preData.getReps());

        Object savedData = new HashMap<String, Object>() {{
            put("sessionId", sessionId);
            put("timestamp", LocalDateTime.now().format(DateTimeFormatter.ISO_DATE_TIME));
            put("data", postData);
        }};

        String filename = sessionId + ".json";
        Path filepath = savePath.resolve(filename);
        try {
            objectMapper.writerWithDefaultPrettyPrinter().writeValue(filepath.toFile(), savedData);
            activeSessions.remove(sessionId);
            System.out.println("Session data saved to: " + filepath.toAbsolutePath());
        } catch (IOException e) {
            System.err.println("File error: " + e.getMessage());
            return new ResponseEntity<>("Server error", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        return new ResponseEntity<>(
                new ResponseStatus("success", "Session data saved", sessionId),
                HttpStatus.OK
        );
    }

    @GetMapping("/results")
    public ResponseEntity<?> getSessionResults() {
        try {
            File[] files = savePath.toFile().listFiles((dir, name) -> name.endsWith(".json"));
            if (files == null || files.length == 0) {
                return new ResponseEntity<>("No session data", HttpStatus.NOT_FOUND);
            }

            File lastFile = java.util.Arrays.stream(files)
                    .max(Comparator.comparingLong(File::lastModified))
                    .orElseThrow(() -> new IOException("Couldn't find newest data"));

            GameSessionData sessionData;
            String sessionId;
            try {
                @SuppressWarnings("unchecked")
                HashMap<String, Object> fileContent = objectMapper.readValue(lastFile, HashMap.class);
                sessionId = (String) fileContent.get("sessionId");

                sessionData = objectMapper.convertValue(fileContent.get("data"), GameSessionData.class);

            } catch (Exception e) {
                System.err.println("File error: " + lastFile.getName() + " - " + e.getMessage());
                return new ResponseEntity<>("Error while converting session file", HttpStatus.INTERNAL_SERVER_ERROR);
            }

            // generate results
            ResultSummary summary = calculateSummary(sessionData);
            summary.setSessionId(sessionId);

            return new ResponseEntity<>(summary, HttpStatus.OK);

        } catch (IOException e) {
            return new ResponseEntity<>("Server error: " + e.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // Returns ResultSummary
    private ResultSummary calculateSummary(GameSessionData session) {
        ResultSummary summary = new ResultSummary();
        List<ControllerFrame> controllersData = session.getControllersData();

        if (controllersData == null || controllersData.isEmpty()) {
            summary.setSummaryTable("No controller data");
            summary.setOverallAccuracy(0.0f);
            return summary;
        }

        StringBuilder output = new StringBuilder("Rep\tPhase\tSliderTime(s)\tLeftHand\tRightHand\n");

        float inhaleTime = session.getInhale();
        float exhaleTime = session.getExhale();
        int reps = session.getReps();

        int frameIndex = 0;
        int totalChecks = 0;
        int totalCorrect = 0;

        for (int r = 0; r < reps; r++) {
            // INHALE
            float inhaleStart = r * (inhaleTime + exhaleTime);
            float inhaleEnd = inhaleStart + inhaleTime;
            boolean leftMatchInhale = false;
            boolean rightMatchInhale = false;

            //int startFrame = frameIndex;
            while (frameIndex < controllersData.size() &&
                    controllersData.get(frameIndex).getTime() <= inhaleEnd) {
                ControllerFrame frame = controllersData.get(frameIndex);
                if (frame.getLeftY() > 0.5f) leftMatchInhale = true;
                if (frame.getRightY() > 0.5f) rightMatchInhale = true;
                frameIndex++;
            }

            output.append(String.format("%d\tInhale\t%.2f\t%s\t%s\n",
                    r + 1, inhaleTime, (leftMatchInhale ? "\t\tV" : "\t\tX"), (rightMatchInhale ? "\tV" : "\tX")));

            totalChecks += 2;
            if (leftMatchInhale) totalCorrect++;
            if (rightMatchInhale) totalCorrect++;

            // EXHALE
            float exhaleEnd = inhaleEnd + exhaleTime;
            boolean leftMatchExhale = false;
            boolean rightMatchExhale = false;

            while (frameIndex < controllersData.size() &&
                    controllersData.get(frameIndex).getTime() <= exhaleEnd) {
                ControllerFrame frame = controllersData.get(frameIndex);
                if (frame.getLeftY() < 0.5f) leftMatchExhale = true;
                if (frame.getRightY() < 0.5f) rightMatchExhale = true;
                frameIndex++;
            }

            output.append(String.format("%d\tExhale\t%.2f\t%s\t%s\n",
                    r + 1, exhaleTime, (leftMatchExhale ? "\t\tV" : "\t\tX"), (rightMatchExhale ? "\tV" : "\tX")));

            totalChecks += 2;
            if (leftMatchExhale) totalCorrect++;
            if (rightMatchExhale) totalCorrect++;
        }

        float percent = (totalChecks > 0) ? (float) totalCorrect / totalChecks * 100f : 0f;
        output.append(String.format("\nOverall Accuracy: %.1f%%", percent));

        summary.setSummaryTable(output.toString());
        summary.setOverallAccuracy(percent);
        return summary;
    }

    // Return status
    private record ResponseStatus(String status, String message, String sessionId) {
    }
}
