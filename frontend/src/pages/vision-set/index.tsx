import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import {Session, TrainedModel, VisionSet} from '../../common/types'
import SessionsList from '../../components/sessions-list'
import Combobox from "../../components/combobox";
import axios from "../../shared/axios";

function VisionSetPage() {
  const { id } = useParams();
  const [visionSet, setVisionSet] = useState<VisionSet>()

  useEffect(() => {
    axios().get(`vision-set/${ id }`)
      .then((res) => {
        console.log(res.data)
        setVisionSet(res.data)
      })
      .catch((err) => {

      })
  }, [])

  const getModelsFetchUrl = (): string => {
    return `https://rcvfunctions.azurewebsites.net/api/project/${ visionSet?.ProjectId }/trained-model`
  }

  const onModelChanged = (newModel: TrainedModel) => {
    if (!visionSet) return;

    setVisionSet({
      ...visionSet,
      TrainedModel: newModel
    })
  }

  const update = () => {
    if (!visionSet) return;

    console.log(visionSet)

    axios().put(`vision-set`, {
      id: visionSet.Id,
      name: visionSet.Name,
      sources: visionSet.Sources,
      trainedModelId: visionSet.TrainedModel.Id
    })
      .then((res) => {
        alert("Updated!")
      })
  }

  return (
    <div>
      { id && visionSet && (
        <div>
          <p>{ visionSet?.Name }</p>
          <SessionsList visionSetId={ +id } />
          <Combobox
            selectedOption={ visionSet.TrainedModel }
            fetchUrl={ getModelsFetchUrl() }
            queryableAttribute={ 'Name' }
            onChange={ onModelChanged }
          />
          <button onClick={ update }>
            Update vision set
          </button>
        </div>
      ) }
    </div>
  )
}

export default VisionSetPage
