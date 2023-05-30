import os.path
import sys

import logging
import numpy as np
import cv2
from flask import Flask, request, Response
import io
from PIL import Image
import detect

# construct the argument parse and parse the arguments

confthres = 0.3
nmsthres = 0.1

def image_to_byte_array(image: Image):
    imgByteArr = io.BytesIO()
    image.save(imgByteArr, format='PNG')
    imgByteArr = imgByteArr.getvalue()
    return imgByteArr


# Initialize the Flask application
app = Flask(__name__)


# route http posts to this method
@app.route('/inference', methods=['POST'])
def inference():
    # load our input image and grab its spatial dimensions
    img = Image.open(io.BytesIO(request.files["file"].read()))

    npimg = np.array(img)
    image = npimg.copy()
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

    res = detect.detect(image)

    print(res)

    # image = cv2.cvtColor(res, cv2.COLOR_BGR2RGB)
    # np_img = Image.fromarray(image)
    # img_encoded = image_to_byte_array(np_img)
    return Response(response=res, status=200, mimetype="application/json")

    # start flask app


# route http posts to this method
@app.route('/start', methods=['POST'])
def start():
    model_uri = request.data
    success = detect.start(model_uri)
    return Response(response=str(success), status=200)


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', threaded=True)
