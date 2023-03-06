FROM nvidia/cuda:11.2.2-cudnn8-devel-ubuntu20.04
MAINTAINER Jeroen Bol "jegbol01@gmail.com"
RUN apt-get update -y && apt-get install -y python3-pip python3-dev libsm6 libxext6 libxrender-dev git

WORKDIR home/
RUN git clone https://github.com/pjreddie/darknet
WORKDIR darknet/
RUN sed -i 's/GPU=.*/GPU=1/' Makefile
RUN sed -i 's/CUDNN=.*/CUDNN=1/' Makefile && \
    make

#We copy just the requirements.txt first to leverage Docker cache
COPY ./requirements.txt /app/requirements.txt
WORKDIR /app
RUN pip3 install --upgrade pip
RUN pip3 install -r requirements.txt
COPY . /app

ENTRYPOINT [ "python3" ]
CMD [ "app.py" ]