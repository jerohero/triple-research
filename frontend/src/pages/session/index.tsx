import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { Negotiate } from '../../common/types'
import axios from '../../shared/axios'
import { AxiosResponse } from 'axios'

function SessionPage() {
  const { id } = useParams();
  const [events, setEvents]: any = useState([])

  useEffect(() => {
    axios().post(`session/${ id }/negotiate`).then((res: AxiosResponse<Negotiate>) => {
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
      { !!events && events.length > 0 && (
        <div>
          { events.map((event: any, i: number) =>
            <div key={ i } className="border-y">
              <h3 className="text-green-600">
                Prediction { Date.now() }
              </h3>
              { event.map((prediction: any, i: number) =>
                <div key={ i } className="my-2">
                  <p>{ JSON.stringify(prediction) }</p>
                </div>
              )}
              <br/>
            </div>
          )}
        </div>
      ) }
    </div>
  )
}

export default SessionPage
