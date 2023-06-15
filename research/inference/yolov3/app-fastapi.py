from fastapi import FastAPI, UploadFile, File
from starlette.responses import JSONResponse

app = FastAPI()


@app.post("/ipc-http-fastapi")
async def upload_file(file: UploadFile = File(...)):
    # contents = await file.read()

    return JSONResponse(content={"message": "OK"}, status_code=200)
