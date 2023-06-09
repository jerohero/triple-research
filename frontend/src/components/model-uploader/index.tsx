import React, { useState } from 'react'
import axios from '../../shared/axios'

function ModelUploader(props: { projectId: number }) {
  const [selectedFile, setSelectedFile] = useState<any>(null)
  const [uploadProgress, setUploadProgress] = useState<number>(0)

  const onFileChange = (e: any) => setSelectedFile(e.target.files ? e.target.files[0] : null)

  const uploadChunk: any = async (chunk: any, start: number, end: number, retries = 3) => {
    try {
      await axios().post(`project/${props.projectId}/trained-model`, chunk, {
        headers: {
          "Content-Type": "application/octet-stream",
          "x-chunk-metadata": JSON.stringify({ name: selectedFile.name, size: selectedFile.size })
        }
      })
      const nextChunkStart = start + chunk.byteLength
      setUploadProgress(Math.min(Math.floor((nextChunkStart / selectedFile.size) * 100), 100))
      return end
    } catch (error: any) {
      if (retries > 0 && error.response.status === 500) {
        console.error("File upload failed, retrying...", error)
        await new Promise(r => setTimeout(r, 1000))
        return uploadChunk(chunk, start, end, retries - 1)
      } else {
        throw error
      }
    }
  }

  const onFileUpload = async () => {
    if (!selectedFile) return

    const chunkSize = 1024 * 1024 * 5 // 5MB
    let start = 0

    while (start < selectedFile.size) {
      const end = Math.min(start + chunkSize, selectedFile.size)
      const chunk = await selectedFile.slice(start, end).arrayBuffer()
      start = await uploadChunk(chunk, start, end)
    }
  }

  return (
    <div>
      <input type="file" onChange={onFileChange} />
      <button onClick={onFileUpload}>Upload!</button>
      <div>Upload Progress: {uploadProgress}%</div>
    </div>
  )
}

export default ModelUploader