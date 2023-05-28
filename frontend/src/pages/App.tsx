import React from 'react'
import {BrowserRouter as Router, Route, Routes} from 'react-router-dom'
import SessionPage from './session'
import SessionsPage from "./sessions";
import ProjectsPage from "./projects";
import ProjectPage from "./project";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<ProjectsPage/>} />
        <Route path="/projects/:id" element={<ProjectPage/>} />
        <Route path="/sessions" element={<SessionsPage/>} />
        <Route path="/sessions/:id" element={<SessionPage/>} />
        <Route path="*" element={<SessionPage/>} />
      </Routes>
    </Router>
  )
}

export default App
