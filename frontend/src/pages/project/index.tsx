import React, {useEffect, useState} from 'react'
import axios from 'axios'
import { useParams } from 'react-router-dom'
import { Project, TrainedModel } from '../../common/types'
import ModelUploader from '../../components/model-uploader'
import VisionSetsList from '../../components/vision-sets-list'
import ModelItem from "../../components/model-item";

function ProjectPage() {
  const { id } = useParams();
  const [project, setProject] = useState<Project>()
  const [trainedModels, setTrainedModels] = useState<TrainedModel[]>()

  useEffect(() => {
    axios.get(`http://localhost:7071/api/project/${ id }`)
      .then((res) => {
        setProject(res.data)
      })
      .catch((err) => {

      })

    axios.get(`http://localhost:7071/api/project/${ id }/trained-model`)
      .then((res) => {
        setTrainedModels(res.data)
      })
      .catch((err) => {

      })
  }, [])

  return (
    <div>
      { id && +id && (
        <>
          <p>{ project?.Name }</p>
          <ModelUploader projectId={ +id } />

          { trainedModels?.map(model => {
            return <ModelItem key={ model.Id } model={ model } />
          }) }

          <VisionSetsList projectId={ +id } />
        </>
      ) }
    </div>
  )
}

export default ProjectPage
