import NewLink from './components/NewLink';
import AllLinks from './components/LinksList';
import Home from './components/Home';
import { Routes, Route, Navigate, BrowserRouter } from "react-router-dom"
import './App.css'

function App() {

  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Home/>}>
            <Route path="all-links" element={<AllLinks/>} />
            <Route path="new" element={<NewLink/>} />
            <Route path="*" element={<Navigate to="/" />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </>
  )
}

export default App
