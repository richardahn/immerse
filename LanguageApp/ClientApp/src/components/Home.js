import React, { Component, useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import { Toast, ToastBody, ToastHeader, Input, InputGroup, Button, Spinner, Tooltip } from 'reactstrap';


function Cloze({ answer, index }) {
  const [tooltipOpen, setTooltipOpen] = useState(false);
  const toggle = () => setTooltipOpen(!tooltipOpen);
  return (
    <>
      <span id={`cloze-${index}`}>_____</span>
      <Tooltip placement="top" isOpen={tooltipOpen} target={`cloze-${index}`} toggle={toggle}>
        {answer}
      </Tooltip>
    </>
  )
}

function mapWords(words, i) {
  return words.map((word, j) => word.cloze
    ?
    <div key={j} style={{ display: 'inline-block' }}>
      <Cloze answer={word.text} index={`${i}-${j}`}/>
      <span style={{ whiteSpace: 'pre' }}> </span>
    </div>
    :
    <span key={j}>{word.text}<span style={{ whiteSpace: 'pre' }}> </span></span>
  )
}
function ConversationCloze({ data }) {
  return (
    <>
      <div>Source: {data.source}</div>
      <div style={{
        display: 'flex',
        flexDirection: 'column',
      }}>
        <div style={{ display: 'flex' }}>
          <div style={{ flex: 1 }}>
            <h5>English parts</h5>
          </div>
          <div style={{ flex: 1 }}>
            <h5>Translated parts</h5>
          </div>
        </div>
        {
          data.lines.map((line, i) =>
            <div key={i} style={{ display: 'flex' }}>
              <div style={{ flex: 1 }}>
                {mapWords(line.nativeWords, i)}
              </div>
              <div style={{ flex: 1 }}>
                {mapWords(line.translatedWords, i)}
              </div>
            </div>
          )
        }
      </div>
    </>
  )
}
export function Home() {
  const [showErrorToast, setShowErrorToast] = useState(false);
  const [conversationCloze, setConversationCloze] = useState(null);
  const [showSpinner, setShowSpinner] = useState(false);
  const [numClozes, setNumClozes] = useState(1);
  const getRandomConversationCloze = useCallback(() => {
    setShowSpinner(true);
    axios
      .get('/api/language/random', {
        params: {
          clozesPerLine: numClozes,
        }
      })
      .then((response) => {
        console.log(response);
        setConversationCloze(response.data);
      }, (error) => {
        setShowErrorToast(true);
      })
      .finally(() => setShowSpinner(false));
  }, [numClozes])

  useEffect(getRandomConversationCloze, [])
  return (
    <div>
      <Toast isOpen={showErrorToast} >
        <ToastHeader toggle={() => setShowErrorToast(!showErrorToast)}>Error</ToastHeader>
        <ToastBody>Failed to reach api</ToastBody>
      </Toast>
      <h4>Learn Korean</h4>
      <Button color="primary" onClick={getRandomConversationCloze}>Refresh</Button>
      <div>Number of clozes</div>
      <InputGroup style={{ marginBottom: '20px' }}>
        <Input placeholder="# of clozes" min={1} max={100} type="number" step="1" value={numClozes} onChange={(event) => setNumClozes(event.target.value)} />
      </InputGroup>
      {showSpinner && <Spinner color="primary" />}
      {conversationCloze && <ConversationCloze data={conversationCloze} />}
    </div>
  )
}