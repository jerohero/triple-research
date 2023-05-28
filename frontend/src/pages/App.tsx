import React from 'react'
import {BrowserRouter as Router, Route, Routes} from 'react-router-dom'
import SessionPage from './session'
import ProjectsPage from "./projects";
import ProjectPage from "./project";
import VisionSetPage from "./vision-set";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<ProjectsPage/>} />
        <Route path="/projects/:id" element={<ProjectPage/>} />
        <Route path="/vision-sets/:id" element={<VisionSetPage/>} />
        <Route path="/sessions/:id" element={<SessionPage/>} />
      </Routes>
    </Router>
  )
}

export default App
