import React, { useEffect, useState } from 'react'
import axios from 'axios'
import { useParams } from 'react-router-dom'
import { Session, VisionSet } from '../../common/types'
import SessionsList from '../../components/sessions-list'

function VisionSetPage() {
  const { id } = useParams();
  const [visionSet, setVisionSet] = useState<VisionSet>()
  const [sessions, setSessions] = useState<Session[]>()

  useEffect(() => {
    axios.get(`http://localhost:7071/api/vision-set/${ id }`)
      .then((res) => {
        setVisionSet(res.data)
      })
      .catch((err) => {

      })

    axios.get(`http://localhost:7071/api/vision-set/${ id }/session`)
      .then((res) => {
        setSessions(res.data)
      })
      .catch((err) => {

      })
  }, [])

  return (
    <div>
      { id && +id && (
        <>
          <p>{ visionSet?.Name }</p>
          <SessionsList visionSetId={ +id } />
        </>
      ) }
    </div>
  )
}

export default VisionSetPage
