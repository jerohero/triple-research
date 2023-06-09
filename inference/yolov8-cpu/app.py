import io
import json

import requests
from PIL import Image
from flask import Flask, Response, request
from ultralytics import YOLO

app = Flask(__name__)

model = None


@app.route('/inference', methods=['POST'])
def inference():
    # load our input image and grab its spatial dimensions
    img = Image.open(io.BytesIO(request.files["file"].read()))

    results = model(img)

    detections = []

    for result in results:
        for box in result.boxes:
            bbox = box.xyxy[0]

            detection = {
                "x1": float(bbox[0]),
                "y1": float(bbox[1]),
                "x2": float(bbox[2]),
                "y2": float(bbox[3]),
                "confidence": float(box.conf[0]),
                "class_index": int(box.cls[0]),
                "class_name": model.names[int(box.cls[0])],
            }

            detections.append(detection)

    print(detections)

    return Response(response=json.dumps(detections), status=200, mimetype="application/json")


@app.route('/start', methods=['POST'])
def start():
    global model

    # model_uri = request.data
    wpath = "model.pt"

    # response = requests.get(model_uri, stream=True)
    # with open(wpath, "wb") as f:
    #     for chunk in response.iter_content(chunk_size=8192):
    #         f.write(chunk)

    model = YOLO(wpath)

    return Response(response=str('Model loaded'), status=200)


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', threaded=True)
