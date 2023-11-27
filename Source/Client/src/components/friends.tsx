import { ToastContainer, Toast, Modal, Button } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import Endpoints from '../endpoints';
import ky, { HTTPError } from 'ky';
import { useState } from 'react';

type friend = {
    id: string;
    displayName: string;
    createdAt: string;
};

export default function Friends() {
    let { error, data } = useQuery<friend[], HTTPError>({
        queryKey: ['data'],
        queryFn: (): Promise<friend[]> =>
            ky.get(Endpoints.Friends.Route()).json(),
    });

    const [showError, setShowError] = useState(false);

    if (error) {
        setShowError(true);
    }

    const [showModal, setShowModal] = useState(false);
    const [selectedFriend, setFriend] = useState<friend | null>(null);

    async function removeFriend() {
        try {
            await ky.post(Endpoints.Friends.Remove(selectedFriend!.id));
            data = data!.filter((friend) => friend.id === selectedFriend?.id);
            setFriend(null);
        } catch (err: unknown) {
            if (err instanceof HTTPError) {
                error = err;
                setShowError(true);
            }
        }
    }

    return (
        <div className="text-center">
            <ToastContainer position="top-end">
                <Toast
                    onClose={() => setShowError(!showError)}
                    show={showError}
                    autohide={true}
                    className="d-inline-block m-1"
                    bg={'danger'}
                >
                    <Toast.Header>
                        <strong className="me-auto">
                            {error?.name || 'Unable to read error name.'}
                        </strong>
                    </Toast.Header>
                    <Toast.Body>
                        {error?.message || 'Unable to read error message.'}
                    </Toast.Body>
                </Toast>
            </ToastContainer>

            <Modal show={showModal}>
                <Modal.Dialog>
                    <Modal.Header
                        closeButton
                        onClick={() => {
                            setShowModal(false);
                            setFriend(null);
                        }}
                    >
                        <Modal.Title>Friend</Modal.Title>
                    </Modal.Header>

                    <Modal.Body>
                        {selectedFriend?.displayName ||
                            "Could not get friend's name."}
                        <div className="fs-6">
                            Created -{' '}
                            {!selectedFriend
                                ? 'Unkown'
                                : new Date(
                                      selectedFriend.createdAt
                                  ).toLocaleDateString('en-US', {
                                      weekday: 'long',
                                      year: 'numeric',
                                      month: 'long',
                                      day: 'numeric',
                                  })}
                        </div>
                    </Modal.Body>

                    <Modal.Footer>
                        <Button variant="danger" onClick={removeFriend}>
                            Remove
                        </Button>
                    </Modal.Footer>
                </Modal.Dialog>
            </Modal>

            <ul className="list-group fs-5">
                {data?.map((friend) => {
                        return (
                            <li
                                className="list-group-item bg-dark-subtle text-primary border-secondary"
                                key={friend.id}
                                onClick={() => {
                                    setFriend(friend);
                                    setShowModal(true);
                                }}
                            >
                                {friend.displayName}
                                <div className="fs-6">
                                    Created -{' '}
                                    {new Date(
                                        friend.createdAt
                                    ).toLocaleDateString('en-US', {
                                        weekday: 'long',
                                        year: 'numeric',
                                        month: 'long',
                                        day: 'numeric',
                                    })}
                                </div>
                            </li>
                        );
                    })}
            </ul>
        </div>
    );
}
