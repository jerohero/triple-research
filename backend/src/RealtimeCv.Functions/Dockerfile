# Stage 0: Setup OpenCV
FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy as builder

ENV OPENCV=4.7.0 DEBIAN_FRONTEND=noninteractive

WORKDIR /

# Install opencv dependencies
RUN apt-get update && apt-get -y install --no-install-recommends \
    apt-transport-https software-properties-common \
    wget unzip ca-certificates build-essential cmake git \
    libtbb-dev libatlas-base-dev libgtk2.0-dev libavcodec-dev \
    libavformat-dev libswscale-dev libdc1394-dev libxine2-dev \
    libv4l-dev libtheora-dev libvorbis-dev libxvidcore-dev \
    libopencore-amrnb-dev libopencore-amrwb-dev x264 \
    libtesseract-dev libgdiplus

RUN apt-get -y clean && rm -rf /var/lib/apt/lists/*

# Setup opencv and opencv-contrib source
RUN wget -q https://github.com/opencv/opencv/archive/$OPENCV.zip && \
    unzip -q $OPENCV.zip && \
    rm $OPENCV.zip && \
    mv opencv-$OPENCV opencv && \
    wget -q https://github.com/opencv/opencv_contrib/archive/$OPENCV.zip && \
    unzip -q $OPENCV.zip && \
    rm $OPENCV.zip && \
    mv opencv_contrib-$OPENCV opencv_contrib

# Build OpenCV
RUN cd opencv && mkdir build && cd build && \
    cmake \
    -D OPENCV_EXTRA_MODULES_PATH=/opencv_contrib/modules \
    -D CMAKE_BUILD_TYPE=RELEASE \
    -D BUILD_SHARED_LIBS=OFF \
    -D ENABLE_CXX11=ON \
    -D BUILD_EXAMPLES=OFF \
    -D BUILD_DOCS=OFF \
    -D BUILD_PERF_TESTS=OFF \
    -D BUILD_TESTS=OFF \
    -D BUILD_JAVA=OFF \
    -D BUILD_opencv_app=OFF \
    -D BUILD_opencv_barcode=OFF \
    -D BUILD_opencv_java_bindings_generator=OFF \
    -D BUILD_opencv_js_bindings_generator=OFF \
    -D BUILD_opencv_python_bindings_generator=OFF \
    -D BUILD_opencv_python_tests=OFF \
    -D BUILD_opencv_ts=OFF \
    -D BUILD_opencv_js=OFF \
    -D BUILD_opencv_bioinspired=OFF \
    -D BUILD_opencv_ccalib=OFF \
    -D BUILD_opencv_datasets=OFF \
    -D BUILD_opencv_dnn_objdetect=OFF \
    -D BUILD_opencv_dpm=OFF \
    -D BUILD_opencv_fuzzy=OFF \
    -D BUILD_opencv_gapi=OFF \
    -D BUILD_opencv_intensity_transform=OFF \
    -D BUILD_opencv_mcc=OFF \
    -D BUILD_opencv_objc_bindings_generator=OFF \
    -D BUILD_opencv_rapid=OFF \
    -D BUILD_opencv_reg=OFF \
    -D BUILD_opencv_stereo=OFF \
    -D BUILD_opencv_structured_light=OFF \
    -D BUILD_opencv_surface_matching=OFF \
    -D BUILD_opencv_videostab=OFF \
    -D BUILD_opencv_wechat_qrcode=ON \
    -D WITH_ADE=OFF \
    -D OPENCV_ENABLE_NONFREE=ON \
    .. && make -j$(nproc) && make install && ldconfig

# Download OpenCvSharp
RUN git clone https://github.com/shimat/opencvsharp.git && cd opencvsharp

# Install the Extern lib.
RUN mkdir /opencvsharp/make && cd /opencvsharp/make && \
    cmake -D CMAKE_INSTALL_PREFIX=/opencvsharp/make /opencvsharp/src && \
    make -j$(nproc) && make install && \
    rm -rf /opencv && \
    rm -rf /opencv_contrib && \
    cp /opencvsharp/make/OpenCvSharpExtern/libOpenCvSharpExtern.so /usr/lib/

# BUILD
FROM  mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated7.0 AS base
WORKDIR /home/site/wwwroot

FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /src
COPY ["src/RealtimeCv.Functions/RealtimeCv.Functions.csproj", "src/RealtimeCv.Functions/"]
RUN dotnet restore "src/RealtimeCv.Functions/RealtimeCv.Functions.csproj"
COPY . .
WORKDIR "/src/src/RealtimeCv.Functions"
RUN dotnet build "RealtimeCv.Functions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealtimeCv.Functions.csproj" -c Release -o /app/publish

# RUN
FROM base AS final
WORKDIR /home/site/wwwroot

COPY --from=publish /app/publish .
COPY --from=builder /usr/lib /usr/lib

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
  AzureFunctionsJobHost__Logging__Console__IsEnabled=true
  
ENTRYPOINT ["dotnet", "RealtimeCv.Functions.dll"]