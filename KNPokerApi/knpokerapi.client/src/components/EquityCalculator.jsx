import React, { useState } from 'react';
import { Form, Button, Card, Alert, Spinner } from 'react-bootstrap';
import axios from 'axios';
import EquityResults from './EquityResults';
import RangeSelector from './RangeSelector';

const EquityCalculator = () => {
    const [range1, setRange1] = useState('AA');
    const [range2, setRange2] = useState('JTs');
    const [results, setResults] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        setResults(null);

        try {
            const response = await axios.get('https://localhost:7285/api/equity', {
                params: {
                    firstRange: range1,
                    secondRange: range2
                }
            });

            setResults(response.data);
        } catch (err) {
            console.error('Error calculating equity:', err);
            setError(err.response?.data?.error || 'An error occurred while calculating equity');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <Card className="mb-4">
                <Card.Header>Input Hand Ranges</Card.Header>
                <Card.Body>
                    <Form onSubmit={handleSubmit}>
                        <Form.Group className="mb-3">
                            <Form.Label>Range 1</Form.Label>
                            <div className="d-flex">
                                <Form.Control
                                    type="text"
                                    value={range1}
                                    onChange={(e) => setRange1(e.target.value)}
                                    placeholder="AA, KK-QQ, AKs"
                                />
                                <RangeSelector onRangeSelect={(range) => setRange1(range)} />
                            </div>
                            <Form.Text className="text-muted">
                                Format examples: "AA-99", "AKs", "KJo", "ATs-A5s"
                            </Form.Text>
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Label>Range 2</Form.Label>
                            <div className="d-flex">
                                <Form.Control
                                    type="text"
                                    value={range2}
                                    onChange={(e) => setRange2(e.target.value)}
                                    placeholder="JTs, 87s, 76s"
                                />
                                <RangeSelector onRangeSelect={(range) => setRange2(range)} />
                            </div>
                            <Form.Text className="text-muted">
                                Format examples: "JTs", "87s-65s", "KQo-KJo"
                            </Form.Text>
                        </Form.Group>

                        <Button variant="primary" type="submit" disabled={loading}>
                            {loading ? (
                                <>
                                    <Spinner
                                        as="span"
                                        animation="border"
                                        size="sm"
                                        role="status"
                                        aria-hidden="true"
                                    />
                                    {' '}Calculating...
                                </>
                            ) : (
                                'Calculate Equity'
                            )}
                        </Button>
                    </Form>
                </Card.Body>
            </Card>

            {error && <Alert variant="danger">{error}</Alert>}

            {results && <EquityResults results={results} />}
        </div>
    );
};

export default EquityCalculator;