import React, {useEffect, useState} from "react";
import {VisionSet} from "../../common/types";
import axios from "axios";
import {Link} from "react-router-dom";

function VisionSetsList(props: { projectId: number }) {
  const [visionSets, setVisionSets] = useState<VisionSet[]>([])

  useEffect(() => {
    axios.get(`http://localhost:7071/api/project/${ props.projectId }/vision-set`).then((res) => {
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