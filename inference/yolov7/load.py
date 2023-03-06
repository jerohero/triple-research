import argparse
import time
from pathlib import Path

import cv2
import torch
import torch.backends.cudnn as cudnn
from numpy import random

from models.experimental import attempt_load
from utils.datasets import LoadStreams, LoadImages
from utils.general import check_img_size, check_requirements, check_imshow, non_max_suppression, apply_classifier, \
    scale_coords, xyxy2xywh, strip_optimizer, set_logging, increment_path
from utils.plots import plot_one_box
from utils.torch_utils import select_device, load_classifier, time_synchronized, TracedModel

def load(img_size, weights, device='', trace=True):
    with torch.no_grad():
        # Initialize
        set_logging()
        device = select_device(device)
        half = device.type != 'cpu'  # half precision only supported on CUDA

        # Load model
        model = attempt_load(weights, map_location=device)  # load FP32 model
        stride = int(model.stride.max())  # model stride
        imgsz = check_img_size(img_size, s=stride)  # check img_size

        if trace:
            model = TracedModel(model, device, imgsz)

        if half:
            model.half()  # to FP16

        # # Second-stage classifier
        # classify = False
        # if classify:
        #     modelc = load_classifier(name='resnet101', n=2)  # initialize
        #     modelc.load_state_dict(torch.load('weights/resnet101.pt', map_location=device)['model']).to(device).eval()

        return model, half, stride, device


# if __name__ == '__main__':
#     parser = argparse.ArgumentParser()
#     parser.add_argument('--weights', nargs='+', type=str, default='yolov7.pt', help='model.pt path(s)')
#     parser.add_argument('--img-size', type=int, default=640, help='inference size (pixels)')
#     parser.add_argument('--device', default='', help='cuda device, i.e. 0 or 0,1,2,3 or cpu')
#     parser.add_argument('--no-trace', action='store_true', help='don`t trace model')
#     opt = parser.parse_args()
#     print(opt)
#     #check_requirements(exclude=('pycocotools', 'thop'))
#
#     with torch.no_grad():
#         if opt.update:  # update all models (to fix SourceChangeWarning)
#             for opt.weights in ['yolov7.pt']:
#                 load(opt.img_size, opt.weights, opt.device, opt.trace)
#                 strip_optimizer(opt.weights)
#         else:
#             load(opt.img_size, opt.weights, opt.device, opt.trace)
