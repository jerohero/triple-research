import React, {useEffect, useState} from "react"
import {VisionSet} from '../../common/types'
import {Link} from 'react-router-dom'
import axios from "../../shared/axios";

function VisionSetsList(props: { projectId: number }) {
  const [visionSets, setVisionSets] = useState<VisionSet[]>([])

  useEffect(() => {
    axios().get(`project/${ props.projectId }/vision-set`).then((res) => {
      setVisionSets(res.data)
    }).catch((err) => {

    })
  }, [])

  return (
    <div>
      { visionSets.map((visionSet: VisionSet, i: number)=>
        <div key={ i } className="border-y">
          <p>Id: {visionSet.Id}</p>
          <p>Name: {visionSet.Name}</p>
          <Link to={`/vision-sets/${ visionSet.Id }`}>
            View
          </Link>
        </div>
      )}
    </div>
  )
}

export default VisionSetsList