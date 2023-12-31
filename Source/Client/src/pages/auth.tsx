import { Button } from 'react-bootstrap';
import { useState } from 'react';

import Signin from '../components/auth/signin';
import Signup from '../components/auth/signup';
import Navbar from '../components/navbar';

export default function Auth() {
    const [showSignin, setShowSignin] = useState<boolean>(true);

    return (
        <>
            <Navbar />
            <div className="container">
                <div className="row vh-100 align-items-center justify-content-center m-1 text-center">
                    <div className="col-sm-8 col-md-6 col-lg-4 bg-white rounded shadow">
                        {showSignin ? <Signin /> : <Signup />}
                        <Button
                            className="btn-secondary mt-1 mb-1 bg-bg-secondary-subtle"
                            onClick={() => setShowSignin(!showSignin)}
                        >
                            Click here to {showSignin ? 'signup' : 'signin'}.
                        </Button>
                    </div>
                </div>
            </div>
        </>
    );
}
