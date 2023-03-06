import subprocess
import sys

import logging
from os import getcwd

from flask import Flask, request, Response

app = Flask(__name__)


@app.route('/api/test', methods=['POST'])
def main():
    working_directory = 'C:/Users/jeroe/Documents/GitHub/triple-research/inference/'
    source = "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5"  # "src-traffic.mp4"
    weights = "yolov7.pt"

    algorithm = "yolov7/app.py"

    # Later replace with calling instance
    res = subprocess.run(
        f'python {algorithm} --weights {weights} --conf 0.4 --img-size 640 --source {source}  --nosave --view-img',
        cwd=working_directory)

    return Response(response=res, status=200, mimetype="image/jpeg")

    # start flask app


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0')
