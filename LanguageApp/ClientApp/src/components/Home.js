import React, { Component, useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import { Space, Layout, Select, Spin, Button, Typography, Divider } from 'antd';

const { Option } = Select;
const { Header, Content } = Layout;
const { Text, Title } = Typography;

function isLineCorrect(line) {
  const submission = (line.answerBlocks ?? []).map(block => block.text).join()
  const answer = line.correctTranslatedBlocks.map(block => block.text).join()
  return submission === answer;
}

function addWordBlock(shuffleProblem, i, j) {
  const wordBlockText = shuffleProblem.lines[i].shuffledTranslatedBlocks[j].text;
  return {
    ...shuffleProblem,
    lines: shuffleProblem.lines.map((line, lineIndex) => lineIndex === i ?
      {
        ...line,
        answerBlocks: (line.answerBlocks ?? []).concat({ text: wordBlockText }),
        shuffledTranslatedBlocks: line.shuffledTranslatedBlocks.filter((block, blockIndex) => blockIndex !== j),
      } : line)
  }
}
function removeWordBlock(shuffleProblem, i, j) {
  const wordBlockText = shuffleProblem.lines[i].answerBlocks[j].text;
  return {
    ...shuffleProblem,
    lines: shuffleProblem.lines.map((line, lineIndex) => lineIndex === i ?
      {
        ...line,
        shuffledTranslatedBlocks: line.shuffledTranslatedBlocks.concat({ text: wordBlockText }),
        answerBlocks: line.answerBlocks.filter((block, blockIndex) => blockIndex !== j),
      } : line)
  }
}


function QuizProblem({ quizProblem, onChange }) {
  const checkLines = useCallback(() => {
    onChange({
      ...quizProblem,
      lines: quizProblem.lines.map(line => ({
        ...line,
        correct: isLineCorrect(line)
      }))
    });
  }, [quizProblem])
  return <div style={{ display: 'flex', justifyContent: 'center', flexDirection: 'column' }}>
    <Text style={{ fontSize: '1.5rem', display: 'block' }}>Movie: {quizProblem.movieName}</Text>
    <Text type="secondary">Shuffle the blocks to match</Text>
    {quizProblem.lines.map((line, i) => <div>
      <div style={{ display: 'flex' }}>
        <div style={{ flex: 1, display: 'flex', alignItems: 'center', padding: '0 1rem' }}>
          <Text style={{ paddingLeft: '1rem' ,borderLeft: `0.3rem solid ${line.correct == null ? 'gray' : line.correct ? 'green' : 'red' }`, fontSize: '1.1rem', color: line.correct == null ? 'inherit' : line.correct ? 'green' : 'red' }}>
            {line.nativeBlocks.map((block, j) => block.text + ' ')}
          </Text>
        </div>
        <div style={{ flex: 1 , padding: '0 1rem'}}>
          <div style={{ borderBottom: '1px solid gray' }}>
            {(line.answerBlocks ?? []).length > 0 ? (line.answerBlocks ?? []).map((block, j) => <Button
              onClick={() => onChange(removeWordBlock(quizProblem, i, j))}>{block.text}</Button>)
              : <Text>Answer...</Text>}
          </div>
          <div style={{ marginTop: '1rem' }}>
            {line.shuffledTranslatedBlocks.length > 0 && line.shuffledTranslatedBlocks.map((block, j) => <Button
              onClick={() => onChange(addWordBlock(quizProblem, i, j))}>{block.text}</Button>)
              }
          </div>
          {line.correct != null && 
            <div>
            <Text style={{ color: line.correct ? 'green' : 'red' }}>{line.correctTranslatedBlocks.map(block => block.text + ' ')}</Text>
            </div>
          }
        </div>
      </div>
      <Divider />
    </div>)}
    <Button type="primary" size="large" onClick={checkLines}>Check!</Button>
    </div>
}

export function Home() {
  const [loadingQuizProblem, setLoadingQuizProblem] = useState(true);
  const [quizProblem, setQuizProblem] = useState(null);
  const getQuizProblem = useCallback((random) => {
    (async function () {
      setLoadingQuizProblem(true);
      try {
        const conversationEndpoint = random ? '' : '/' + quizProblem.conversationId;
        const { data } = await axios.get(`/api/language/problem/shuffle${conversationEndpoint}`)
        setQuizProblem(data);
        console.log('quiz problem', data);
      } catch (error) {
        console.log(error, error.response);
      } finally {
        setLoadingQuizProblem(false);
      }
    })();
  }, [quizProblem])
  useEffect(() => getQuizProblem(true), [])
  return (
      <Content style={{ padding: '2rem 3rem' }}>
      <div style={{ display: 'flex', justifyContent: 'center' }}>
        <Space>
          <Button type="primary" onClick={() => getQuizProblem(true)}>Generate New Problem</Button>
          </Space>
        </div>
        <div>
          {loadingQuizProblem ? <Spin /> : quizProblem && <QuizProblem quizProblem={quizProblem} onChange={setQuizProblem} />}
        </div>
      </Content>
  )
}