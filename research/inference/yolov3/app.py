import sys

import logging
import numpy as np
import cv2
from flask import Flask, request, Response
import io
from PIL import Image
import detect

confthres = 0.3
nmsthres = 0.1
yolo_path = './'


def image_to_byte_array(image: Image):
    imgByteArr = io.BytesIO()
    image.save(imgByteArr, format='PNG')
    imgByteArr = imgByteArr.getvalue()
    return imgByteArr


app = Flask(__name__)


@app.route('/inference', methods=['POST'])
def inference():
    # load our input image and grab its spatial dimensions
    img = Image.open(io.BytesIO(request.files["file"].read()))

    npimg = np.array(img)
    image = npimg.copy()
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

    res = detect.detect(image)

    print(res)
    return Response(response=res, status=200, mimetype="image/jpeg")


@app.route('/start', methods=['POST'])
def start():
    success = detect.start()
    return Response(response=str(success), status=200)


# IPC (HTTP)
@app.route('/ipc-http', methods=['POST'])
def inference_empty():
    return Response(response="OK", status=200)


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', threaded=True)
