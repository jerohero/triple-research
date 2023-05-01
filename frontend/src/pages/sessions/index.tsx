import React, {useEffect, useState} from 'react'
import axios from "axios";
import {Link} from "react-router-dom";

interface Session {
  Id: number,
  VisionSetId: number,
  IsActive: boolean,
  Source: string,
  Pod: string,
  CreatedAt: Date,
  StartedAt: Date,
  StoppedAt: Date
}

function Sessions() {
  const [sessions, setSessions] = useState<Session[]>([])

  useEffect(() => {
    axios.get('http://localhost:7071/api/vision-set/1/session').then((res) => {
      console.log(res.data)

      setSessions(res.data)

      console.log(sessions)
    }).catch((err) => {

    })
  }, [])

  return (
    <div>
      { sessions.map((session: Session, i: number)=>
        <div key={ i } className="border-y">
          <p>Id: {session.Id}</p>
          <p>{session.IsActive ? 'Active' : 'Inactive'}</p>
          <p>Pod: {session.Pod}</p>
          <Link to={`/session/${ session.Id }`}>View</Link>
        </div>
      )}
    </div>
  )
}

export default Sessions
