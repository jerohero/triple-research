import React, {useEffect, useState} from 'react'
import './App.css'

function App() {
  const link: string = 'wss://triplejbpubsub.webpubsub.azure.com/client/hubs/predictions?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2Nzg4MDcxMzcsImV4cCI6MTY3OTA2NjMzNywiaWF0IjoxNjc4ODA3MTM3LCJhdWQiOiJodHRwczovL3RyaXBsZWpicHVic3ViLndlYnB1YnN1Yi5henVyZS5jb20vY2xpZW50L2h1YnMvcHJlZGljdGlvbnMifQ.mfaA5Uqx3V_o1QixFvfPbrrTMdmEJwGjZUhfXJtFzts'

  const [events, setEvents]: any = useState([])

  useEffect(() => {
    const ws = new WebSocket(link)

    ws.onopen = () => {
      console.log('Connected!')
    }

    ws.onmessage = (event: MessageEvent) => {
      const data = JSON.parse(event.data)
      setEvents((current: any) => [data, ...current])
    }

  }, [])

  return (
    <div>
      { events.map((event: any, i: number)=>
        <div key={ i } className="border-y">
          <h3>Prediction { Date.now() }</h3>
          { event.predictions.map((prediction: any, i: number) =>
            <div key={ i } className="my-2">
              <p className="text-green-600">{ prediction.label }</p>
              <p>{ prediction.probability }</p>
              <p>{ JSON.stringify(prediction.boundingBox) }</p>
            </div>
          )}
          <br/>
        </div>
      )}
    </div>
  )
}

export default App
