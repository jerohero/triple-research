import React, {useEffect, useState} from 'react'
import { Link } from 'react-router-dom'
import { Project } from '../../common/types'
import axios from "../../shared/axios";

function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([])

  useEffect(() => {
    axios().get('project').then((res) => {
      setProjects(res.data)
    }).catch((err) => {

    })
  }, [])

  return (
    <div>
      { projects.map((project: Project, i: number)=>
        <div key={ i } className="border-y">
          <p>Id: {project.Id}</p>
          <p>Name: {project.Name}</p>
          <Link to={`/projects/${ project.Id }`}>View</Link>
        </div>
      )}
    </div>
  )
}

export default ProjectsPage
