import React, {useEffect} from 'react'
import './App.css'

function App() {
  const link: string = 'wss://triplejbpubsub.webpubsub.azure.com/client/hubs/predictions?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2Nzg4MDcxMzcsImV4cCI6MTY3OTA2NjMzNywiaWF0IjoxNjc4ODA3MTM3LCJhdWQiOiJodHRwczovL3RyaXBsZWpicHVic3ViLndlYnB1YnN1Yi5henVyZS5jb20vY2xpZW50L2h1YnMvcHJlZGljdGlvbnMifQ.mfaA5Uqx3V_o1QixFvfPbrrTMdmEJwGjZUhfXJtFzts'

  useEffect(() => {
    const ws = new WebSocket(link)

    ws.onopen = () => {
      console.log('Connected!')
    }

    ws.onmessage = (event) => {
      console.log(JSON.parse(event.data))
    }

  }, [])

  return (
    <div>

    </div>
  )
}

export default App
