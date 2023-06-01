import React, { useState } from 'react'
import {TrainedModel} from "../../common/types";

function ModelItem(props: { model: TrainedModel }) {
  return (
    <div className='p-2'>
      <div>
        { props.model.Name }
      </div>
    </div>
  )
}

export default ModelItem