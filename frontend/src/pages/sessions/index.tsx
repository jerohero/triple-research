import React, {useEffect, useState} from 'react'
import axios from "axios";
import {Link} from "react-router-dom";
import {Session} from "../../common/types";

function SessionsPage() {
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
          <Link to={`/sessions/${ session.Id }`}>View</Link>
        </div>
      )}
    </div>
  )
}

export default SessionsPage
