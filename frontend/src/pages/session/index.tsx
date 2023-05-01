import React, { useEffect, useState } from 'react'
import axios, {AxiosResponse} from 'axios'
import { useParams } from 'react-router-dom'

interface Negotiate {
  Url: string
}

function Session() {
  const { id } = useParams();
  const [events, setEvents]: any = useState([])

  useEffect(() => {
    axios.post(`http://localhost:7071/api/session/${ id }/negotiate`).then((res: AxiosResponse<Negotiate>) => {
      console.log(res.data.Url)
      const ws = new WebSocket(res.data.Url)

      ws.onopen = () => {
        console.log('Connected!')
      }

      ws.onmessage = (event: MessageEvent) => {
        const data = JSON.parse(event.data)
        setEvents((current: any) => [data, ...current])
      }
    })
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

export default Session
