import React from 'react';
import { ThemeProvider } from 'styled-components';
import ChatBot from '../../lib/index';
import SessionResponse from './SessionResponse.jsx';
import Game from './Game';


const otherFontTheme = {
  background: '#f5f8fb',
  fontFamily: 'Quicksand,sans-serif',
  headerBgColor: '#6e48aa',
  headerFontColor: '#fff',
  headerFontSize: '16px',
  botBubbleColor: '#6E48AA',
  botFontColor: '#fff',
  userBubbleColor: '#fff',
  userFontColor: '#4a4a4a'
};

const steps = [
    {
      id: '1',
      message: 'Hi, how are we feeling today?',
      trigger: '2',
    },
    {
      id: '2',
      options: [
          { value: 1, label: 'Great', trigger: '3' },
          { value: 2, label: 'So so', trigger: '4' },
          { value: 3, label: 'Not good', trigger: '4' }
        ]
    },
    {
      id: '3',
      message: 'Glad to here it.',
      trigger: '6',
    },
    {
      id: '6',
      message: "Let's use our time than and learn more.",
      trigger: '7'
    },
    {
      id: '6',
      message: "Oh my. Ok, let\'s try to make it better.",
      trigger: '7'
    },
    {
      id: '7',
      options: [
          { value: 1, label: 'Healthy diet tips', trigger: '8' },
          { value: 3, label: 'Watch info videos', trigger: '10' },
          { value: 4, label: 'Cognie Game', trigger: 'game_start' },
        ]
    },
    {
      id: '8',
      message: 'While there is no magic diet, there are some good food: all variety of fruits and veg, fiber rich Whole grain, skinless poultry and fish (fish at least twice weekly), non tropical vegetable cooking oils (e.g. canola, corn, olive, peanut, soybean, sunflower), Low fat (1%) or fat free (skim) dairy products and low sodium foods (cook w/o or w/ little added salt).',
      trigger: '9'
    },
    {
      id: '9',
      message: 'Foods to "Limit/Avoid": These include foods with high saturated fats (fatty beef, lamb, port, poultry w/ skin, beef fat, lard, cream, butter, cheese). If you to have red meat, choose the leanest cut. This section also includes foods high in trans fats: This includes things like doughnuts, cookies, crackers, muffins, pies & cakes, commercially fried foods /baked goods with shortening or partially hydrogenated vegetable oils. It is important to avoid sweets and sugar sweetened beverages. Also important to avoid foods high in sodium. prepare foods with little/no salt.',
      delay: 5000,
      trigger: '12'
    },
    {
        id: '4',
        message: 'Ok, let\'s try to make it better than. How about trying some of these?',
        trigger: '7'
    },
    {
      id: '10',
      options: [
          { value: 1, label: 'Healthy diet', trigger: '5' },
          { value: 2, label: 'Managing Multiple Sclerosis', trigger: '11' },
          { value: 3, label: 'Cognie Game', trigger: 'game_start' },
        ]
    },
    {
        id: '5',
        component: (
            <iframe width="560" height="315" src="https://www.youtube.com/embed/3M5pSb9SRJU" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
        ),
        trigger: 'delayed'
    },
    {
        id: '11',
        component: (
            <iframe width="560" height="315" src="https://www.youtube.com/embed/3wrWudX9Kh0" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
        ),
        trigger: 'delayed'
    },
    {
      id: 'delayed',
      options: [
          { value: 1, label: 'Let\'s continue', trigger: '7' },
          { value: 2, label: 'Thanks, enough for today.', trigger: '13' }
      ],
      delay: 20000
    },
    {
      id: '12',
      options: [
          { value: 1, label: 'Let\'s continue', trigger: '7' },
          { value: 2, label: 'Thanks, enough for today.', trigger: '13' }
        ]
    },
    {
      id: '13',
      message: 'See you soon. I just leave here a few tips.',
      trigger: '7'
    },
    {
      id: 'game_start',
      component: (
          <a href="http://scholtz.sk:8080/" target="_blank">So let's start! Tap here!</a>
        ),
      trigger: 'delayed',
    },
    {
      id: 'session_id',
      user: true,
      trigger: 'session_id_response',
    },
    {
      id: 'session_id_response',
      component: <SessionResponse />,
      waitAction: true,
      trigger: 'cognie_game',
    },
    {
      id: 'cognie_game',
      component: (
          <a href="http://scholtz.sk:8080/" target="_blank">START</a>
        ),
      waitAction: true,
      trigger: '1',
      end: true,
    },
];

const ThemedExample = () => (
  <ThemeProvider theme={otherFontTheme}>
    <React.StrictMode>
      <ChatBot steps={steps} headerTitle="CognieApp" recognitionEnable={true} />
    </React.StrictMode>
  </ThemeProvider>
);

export default ThemedExample;
