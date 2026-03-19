// src/pages/victim/MyRequestsPage.jsx
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AppHeader from '../../components/AppHeader/AppHeader';
import { reliefRequestsApi } from '../../api/reliefRequestsApi';

const COLORS = {
    Open:      { bg: '#e8f5e9', color: '#2e7d32' },
    Assigned:  { bg: '#fff3e0', color: '#e65100' },
    Completed: { bg: '#ede7f6', color: '#4527a0' },
};

export default function MyRequestsPage() {
    const nav = useNavigate();
    const [requests, setRequests]   = useState([]);
    const [loading, setLoading]     = useState(true);
    const [error, setError]         = useState(null);

    useEffect(() => {
        reliefRequestsApi.getAll()
            .then(r => setRequests(r.data))
            .catch(() => setError('Failed to load requests.'))
            .finally(() => setLoading(false));
    }, []);

    return (
        <>
            <AppHeader />
            <div style={pageWrap}>
                <div style={card}>

                    {/*header*/}
                    <div style={headerRow}>
                        <div>
                            <h2 style={title}>My Requests</h2>
                            <p style={subtitle}>Track the status of your submitted relief requests.</p>
                        </div>
                        <button style={primBtn}
                            onClick={() => nav('/victim/new-request')}>
                            + New Request
                        </button>
                    </div>

                    <hr style={divider} />

                    {/*states*/}
                    {loading && <p style={muted}>Loading...</p>}
                    {error   && <p style={errorTxt}>{error}</p>}
                    {!loading && requests.length === 0 && (
                        <div style={emptyBox}>
                            <p style={{ fontSize: 32, margin: 0 }}>📋</p>
                            <p style={{ color: '#888', margin: '8px 0 0' }}>
                                No requests yet. Submit your first request.</p>
                        </div>
                    )}

                    {/*cards*/}
                    {requests.map(r => (
                        <div key={r.requestId} style={reqCard}>
                            <div style={reqTop}>
                                <div>
                                    <p style={areaText}>{r.area}</p>
                                    <p style={descText}>
                                        {r.description ?? 'No description provided.'}
                                    </p>
                                </div>
                                <span style={{ ...badge, ...COLORS[r.status] }}>
                                    {r.status}
                                </span>
                            </div>
                            <div style={reqFooter}>
                                <span>Urgency: <b>{r.urgency}</b></span>
                                <span>Submitted: {new Date(r.createdAt).toLocaleDateString()}</span>
                            </div>
                        </div>
                    ))}

                    <button style={secBtn} onClick={() => nav('/victim')}>
                        ← Back to Dashboard
                    </button>

                </div>
            </div>
        </>
    );
}

// styles
const pageWrap  = { minHeight: '100vh', background: '#f5f6fa',
                    display: 'flex', justifyContent: 'center',
                    padding: '48px 16px',
                    fontFamily: "'Segoe UI', Inter, sans-serif" };
const card      = { background: '#fff', borderRadius: 16,
                    boxShadow: '0 4px 24px rgba(0,0,0,0.08)',
                    padding: '36px 40px', width: '100%', maxWidth: 620 };
const headerRow = { display: 'flex', justifyContent: 'space-between',
                    alignItems: 'flex-start', gap: 16 };
const title     = { margin: 0, fontSize: 22, fontWeight: 700,
                    color: '#1a1a2e', letterSpacing: '-0.3px' };
const subtitle  = { margin: '4px 0 0', fontSize: 13, color: '#888' };
const divider   = { border: 'none', borderTop: '1px solid #f0f0f0',
                    margin: '20px 0 24px' };
const muted     = { color: '#888', textAlign: 'center', padding: '32px 0' };
const errorTxt  = { color: '#c0392b', fontSize: 13 };
const emptyBox  = { textAlign: 'center', padding: '40px 0' };
const reqCard   = { border: '1px solid #f0f0f0', borderRadius: 12,
                    padding: '16px 20px', marginBottom: 12,
                    background: '#fafafa' };
const reqTop    = { display: 'flex', justifyContent: 'space-between',
                    alignItems: 'flex-start', gap: 12 };
const areaText  = { margin: 0, fontWeight: 700, fontSize: 15,
                    color: '#1a1a2e' };
const descText  = { margin: '4px 0 0', fontSize: 13, color: '#666' };
const reqFooter = { display: 'flex', justifyContent: 'space-between',
                    marginTop: 12, fontSize: 12, color: '#999' };
const badge     = { fontSize: 11, fontWeight: 700, padding: '4px 12px',
                    borderRadius: 20, whiteSpace: 'nowrap',
                    flexShrink: 0 };
const primBtn   = { padding: '10px 18px', borderRadius: 10, border: 'none',
                    background: '#1a1a2e', color: '#fff',
                    cursor: 'pointer', fontSize: 13, fontWeight: 600,
                    fontFamily: "'Segoe UI', Inter, sans-serif" };
const secBtn    = { width: '100%', padding: 12, borderRadius: 10,
                    border: '1.5px solid #e8e8e8', marginTop: 16,
                    background: '#fff', color: '#555',
                    cursor: 'pointer', fontSize: 14,
                    fontFamily: "'Segoe UI', Inter, sans-serif" };