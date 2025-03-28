import React from 'react';
import { Container } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import EquityCalculator from './components/EquityCalculator';
import './App.css';

function App() {
    return (
        <div className="App">
            <Container>
                <header className="App-header my-4">
                    <h1>Poker Equity Calculator</h1>
                    <p>Calculate the equity between two poker hand ranges</p>
                </header>
                <main>
                    <EquityCalculator />
                </main>
                <footer className="my-4 text-center text-muted">
                    <p>Powered by KNPoker</p>
                </footer>
            </Container>
        </div>
    );
}

export default App;