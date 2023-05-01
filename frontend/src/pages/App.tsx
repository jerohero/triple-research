import React from 'react'
import {BrowserRouter as Router, Route, Routes} from 'react-router-dom'
import Session from './session'
import Sessions from "./sessions";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Sessions/>} />
        <Route path="/session/:id" element={<Session/>} />
        <Route path="*" element={<Session/>} />
      </Routes>
    </Router>
  )
}

export default App
