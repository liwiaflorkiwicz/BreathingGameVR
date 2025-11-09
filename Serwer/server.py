from flask import Flask, request, jsonify
import os, json, uuid
from datetime import datetime

app = Flask(__name__)

SAVE_DIR = "sessions"
os.makedirs(SAVE_DIR, exist_ok=True)

@app.route("/api/data", methods=["POST"])
def receive_data():
    """
    Odbiera JSON z Unity, zapisuje do pliku.
    Dodaje unikalne sessionId i timestamp.
    Pole 'data' zawiera m.in dane z kontrolerów
    """
    data = request.get_json()
    if not data:
        return jsonify({"error": "No JSON received"}), 400

    # Dodaj sessionId i timestamp
    session_id = str(uuid.uuid4())
    timestamp = datetime.now().isoformat()

    # Tworzymy strukturę, w której dane z Unity trafiają do pola 'data'
    saved_data = {
        "sessionId": session_id,
        "timestamp": timestamp,
        "data": data  # przesyłmy obiekty z kontrolerów jako JSON
    }

    # Zapis do pliku
    filename = f"{session_id}.json"
    filepath = os.path.join(SAVE_DIR, filename)
    with open(filepath, "w", encoding="utf-8") as f:
        json.dump(saved_data, f, ensure_ascii=False, indent=4)

    print(f"Data saved to: {filepath}")
    return jsonify({"status": "success", "sessionId": session_id}), 200

# @app.route("/api/data", methods=["GET"])
# def get_all_data():
#     files = os.listdir(SAVE_DIR)
#     all_data = []
#     for file in files:
#         if file.endswith(".json"):
#             with open(os.path.join(SAVE_DIR, file), "r", encoding="utf-8") as f:
#                 all_data.append(json.load(f))
#     return jsonify(all_data), 200

@app.route("/api/data", methods=["GET"])
def get_last_session():
    files = [f for f in os.listdir(SAVE_DIR) if f.endswith(".json")]
    if not files:
        return jsonify({"error": "No session data"}), 404

    # sorting based on last change time
    files.sort(key=lambda x: os.path.getmtime(os.path.join(SAVE_DIR, x)), reverse=True)
    last_file = files[0]

    with open(os.path.join(SAVE_DIR, last_file), "r", encoding="utf-8") as f:
        data = json.load(f)

    return jsonify(data["data"]), 200


if __name__ == "__main__":
    # Tryb developerski - Flask działa lokalnie
    app.run(host="0.0.0.0", port=5000, debug=True)
