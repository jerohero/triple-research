import React, {useEffect, useState} from 'react'
import './App.css'

function App() {
  const link: string = '' // For now, put the outputted ws:// URL from the worker here

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
