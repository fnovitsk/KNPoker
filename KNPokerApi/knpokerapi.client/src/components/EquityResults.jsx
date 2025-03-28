import React from 'react';
import { Card, Table, Alert } from 'react-bootstrap';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip, Legend } from 'recharts';

const EquityResults = ({ results }) => {
    if (!results || !results.summary) {
        return <Alert variant="info">No results to display</Alert>;
    }

    // Extract summary data for the chart
    const chartData = [
        { name: "Range 1 Wins", value: results.summary.find(s => s.label === "Range 1 Wins")?.value || 0 },
        { name: "Range 2 Wins", value: results.summary.find(s => s.label === "Range 2 Wins")?.value || 0 },
        { name: "Tie", value: results.summary.find(s => s.label === "Ranges Tie")?.value || 0 }
    ];

    const COLORS = ['#0088FE', '#00C49F', '#FFBB28'];

    return (
        <div>
            <Card className="mb-4">
                <Card.Header>Equity Results</Card.Header>
                <Card.Body>
                    <div className="row">
                        <div className="col-md-6">
                            <h5>Summary</h5>
                            <Table striped bordered hover>
                                <thead>
                                    <tr>
                                        <th>Result</th>
                                        <th>Equity (%)</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {results.summary.map((item, index) => (
                                        <tr key={index}>
                                            <td>{item.label}</td>
                                            <td>{(item.value * 100).toFixed(2)}%</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </Table>
                        </div>
                        <div className="col-md-6">
                            <h5>Equity Distribution</h5>
                            <ResponsiveContainer width="100%" height={300}>
                                <PieChart>
                                    <Pie
                                        data={chartData}
                                        cx="50%"
                                        cy="50%"
                                        labelLine={false}
                                        outerRadius={100}
                                        fill="#8884d8"
                                        dataKey="value"
                                        label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(2)}%`}
                                    >
                                        {chartData.map((entry, index) => (
                                            <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                                        ))}
                                    </Pie>
                                    <Tooltip
                                        formatter={(value) => `${(value * 100).toFixed(2)}%`}
                                    />
                                    <Legend />
                                </PieChart>
                            </ResponsiveContainer>
                        </div>
                    </div>
                </Card.Body>
            </Card>

            <Card>
                <Card.Header>Detailed Results</Card.Header>
                <Card.Body>
                    <Table striped bordered hover responsive>
                        <thead>
                            <tr>
                                <th>Hand 1</th>
                                <th>Hand 1 Equity</th>
                                <th>Hand 2</th>
                                <th>Hand 2 Equity</th>
                                <th>Tie Equity</th>
                                <th>Combos</th>
                            </tr>
                        </thead>
                        <tbody>
                            {results.detailedResults && results.detailedResults.map((result, index) => (
                                <tr key={index}>
                                    <td>{result.hand1}</td>
                                    <td>{(result.hand1Equity * 100).toFixed(2)}%</td>
                                    <td>{result.hand2}</td>
                                    <td>{(result.hand2Equity * 100).toFixed(2)}%</td>
                                    <td>{(result.tieEquity * 100).toFixed(2)}%</td>
                                    <td>{result.combos}</td>
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                </Card.Body>
            </Card>
        </div>
    );
};

export default EquityResults;