import React, { useState } from 'react';
import { Button, Modal, Table } from 'react-bootstrap';

const RangeSelector = ({ onRangeSelect }) => {
    const [show, setShow] = useState(false);
    const [selectedCells, setSelectedCells] = useState(new Set());

    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);

    const handleApply = () => {
        // Convert selected cells to range notation
        const rangeString = Array.from(selectedCells).join(', ');
        onRangeSelect(rangeString);
        handleClose();
    };

    const toggleCell = (hand) => {
        const newSelected = new Set(selectedCells);
        if (newSelected.has(hand)) {
            newSelected.delete(hand);
        } else {
            newSelected.add(hand);
        }
        setSelectedCells(newSelected);
    };

    // Create the grid of hands
    const ranks = ['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];

    return (
        <>
            <Button variant="outline-secondary" onClick={handleShow} className="ms-2">
                Grid
            </Button>

            <Modal show={show} onHide={handleClose} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Select Hand Range</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Table bordered className="hand-grid">
                        <thead>
                            <tr>
                                <th></th>
                                {ranks.map(rank => (
                                    <th key={rank} className="text-center">{rank}</th>
                                ))}
                            </tr>
                        </thead>
                        <tbody>
                            {ranks.map((rank1, i) => (
                                <tr key={rank1}>
                                    <th className="text-center">{rank1}</th>
                                    {ranks.map((rank2, j) => {
                                        let hand;
                                        let cellClass;

                                        if (i === j) {
                                            // Pair
                                            hand = `${rank1}${rank1}`;
                                            cellClass = 'table-warning';
                                        } else if (i < j) {
                                            // Suited hand
                                            hand = `${rank1}${rank2}s`;
                                            cellClass = 'table-info';
                                        } else {
                                            // Offsuit hand
                                            hand = `${rank2}${rank1}o`;
                                            cellClass = 'table-light';
                                        }

                                        const isSelected = selectedCells.has(hand);

                                        return (
                                            <td
                                                key={`${rank1}${rank2}`}
                                                className={`text-center ${cellClass} ${isSelected ? 'selected-hand' : ''}`}
                                                onClick={() => toggleCell(hand)}
                                                style={{
                                                    cursor: 'pointer',
                                                    backgroundColor: isSelected ? '#007bff' : '',
                                                    color: isSelected ? 'white' : ''
                                                }}
                                            >
                                                {hand}
                                            </td>
                                        );
                                    })}
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={handleApply}>
                        Apply Selection
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default RangeSelector;