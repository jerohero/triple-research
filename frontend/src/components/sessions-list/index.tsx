import React, { useEffect, useState } from "react"
import { Session } from '../../common/types'
import { Link } from 'react-router-dom'
import axios from "../../shared/axios";

function SessionsList(props: { visionSetId: number }) {
  const [sessions, setSessions] = useState<Session[]>([])

  useEffect(() => {
    axios().get(`vision-set/${ props.visionSetId }/session`).then((res) => {
      setSessions(res.data)
    }).catch((err) => {

    })
  }, [])

  return (
    <div>
      { sessions.map((session: Session, i: number)=>
        <div key={ i } className="border-y">
          <p>Id: {session.Id}</p>
          <p>Name: {session.Pod}</p>
          <Link to={`/sessions/${ session.Id }`}>
            View
          </Link>
        </div>
      )}
    </div>
  )
}

export default SessionsList