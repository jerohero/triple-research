import os

from flask import Flask, Response, request

from detect import detect
from load import load

app = Flask(__name__)

model = None
# modelc = None
half = None
stride = None
# classify = None
img_size = None
device = None


@app.route('/inference', methods=['POST'])
def inference():
    working_directory = 'C:/Users/jeroe/Documents/GitHub/triple-research/inference/yolov7/'
    os.chdir(working_directory)
    # source = "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5"  # "src-traffic.mp4"
    source = request.files['file'].read()
    weights = "yolov7.pt"

    # Later replace with calling instance
    # res = subprocess.run(
    #     f'python {algorithm} --weights {weights} --conf 0.4 --img-size 640 --source {source}  --nosave --view-img',
    #     cwd=working_directory)

    detect(source, img_size, stride, model, device, half)

    return Response(response='ee', status=200, mimetype="image/jpeg")

    # start flask app


@app.route('/start', methods=['POST'])
def start():
    working_directory = 'C:/Users/jeroe/Documents/GitHub/triple-research/inference/yolov7/'
    os.chdir(working_directory)

    global model, half, img_size, stride, device

    weights = "yolov7.pt"
    device = ''
    img_size = 640
    trace = True

    model, half, stride, device = load(img_size, weights, device, trace)

    return Response(response='ok', status=200)


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0')
