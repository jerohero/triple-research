import json
import mmap
import socket
import time
import cv2
import numpy as np

import win32event


# Shared memory
def main_sm():
    image_shared_memory_name = "image"
    result_memory_name = "result"
    image_size_memory_name = "imageSize"
    image_semaphore_name = "imageReady"
    result_semaphore_name = "resultReady"

    result_size = 1024 * 5  # 5 KB

    image_mm = None
    result_mm = None

    # image_size_mm = mmap.mmap(-1, 4, image_size_memory_name)  # w/ size
    # image_size = int.from_bytes(image_size_mm.read(4), byteorder='little')  # w/ size
    image_size = 810341  # preset size (0.81 MB)

    # Open semaphores for signaling
    image_semaphore = win32event.OpenEvent(win32event.EVENT_ALL_ACCESS, False, image_semaphore_name)
    result_semaphore = win32event.OpenEvent(win32event.EVENT_ALL_ACCESS, False, result_semaphore_name)

    while True:
        result = win32event.WaitForSingleObject(image_semaphore, 10000)  # timeout in ms

        if result == win32event.WAIT_OBJECT_0:
            # read image
            # start = time.perf_counter()

            if image_mm is None:
                image_mm = mmap.mmap(-1, image_size, image_shared_memory_name)

            image_mm.seek(0)
            image_bytes = image_mm.read(image_size)

            # frame = cv2.imdecode(np.frombuffer(image_bytes, np.uint8), cv2.IMREAD_COLOR)
            # cv2.imshow("Image", frame)
            # cv2.waitKey()

            if result_mm is None:
                result_mm = mmap.mmap(-1, result_size, result_memory_name)

            result_bytes = json.dumps({"result": "success"}).encode()

            # print(result_bytes)

            result_mm.seek(0)
            result_mm.write(result_bytes)

            win32event.SetEvent(result_semaphore)

            # stop = time.perf_counter()
            # print("Reading Duration:", (stop - start) * 1000, "ms")
    cv2.destroyAllWindows()
    mm.close()


# TCP socket
def main_tcp():
    server_address = ('localhost', 5000)
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.bind(server_address)
    sock.listen(1)

    # use with
    def recvall(input_sock, n):
        data = bytearray()
        while len(data) < n:
            packet = input_sock.recv(n - len(data))
            if not packet:
                return None
            data.extend(packet)
        return data

    try:
        while True:
            print("Waiting for a connection...")
            connection, client_address = sock.accept()

            try:
                print("Connection from", client_address)

                while True:
                    frame_size_bytes = recvall(connection, 4)  # w/ size
                    if not frame_size_bytes:
                        break
                    frame_size = int.from_bytes(frame_size_bytes, 'little')

                    # Read frame data
                    # frame_bytes = connection.recv(810341)  # preset max size (0.81 MB)
                    frame_bytes = recvall(connection, frame_size)  # w/ size
                    if not frame_bytes:
                        break

                    # frame = cv2.imdecode(np.frombuffer(frame_bytes, np.uint8), cv2.IMREAD_COLOR)
                    # cv2.imshow("Image", frame)
                    # cv2.waitKey()

                    result_data = {
                        "result": "success"
                    }

                    # Send result
                    result_bytes = json.dumps(result_data).encode()
                    connection.sendall(result_bytes)

            finally:
                connection.close()

    except Exception as e:
        print("Error: ", str(e))

    sock.close()


if __name__ == "__main__":
    # main_tcp()
    main_sm()
