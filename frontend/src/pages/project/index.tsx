import React, {useEffect, useState} from 'react'
import axios from 'axios'
import {useParams} from 'react-router-dom'
import {Project} from '../../common/types'
import ModelUploader from '../../components/model-uploader'

function ProjectPage() {
  const { id } = useParams();
  const [project, setProject] = useState<Project>()

  useEffect(() => {
    axios.get(`http://localhost:7071/api/project/${ id }`).then((res) => {
      setProject(res.data)
    }).catch((err) => {

    })
  }, [])

  return (
    <div>
      { id && +id && (
        <>
          <p>{ project?.Name }</p>
          <ModelUploader projectId={ +id } />
        </>
      ) }
    </div>
  )
}

export default ProjectPage
