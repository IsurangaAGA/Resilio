import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AppHeader from '../../components/AppHeader/AppHeader';
import { reliefRequestsApi } from '../../api/reliefRequestsApi';

const STATUSES = ['All', 'Open', 'Assigned', 'Completed'];
const URGENCY  = ['Low', 'Medium', 'High', 'Critical'];

const COLORS = {
    Open:      { bg: '#e8f5e9', color: '#2e7d32' },
    Assigned:  { bg: '#fff3e0', color: '#e65100' },
    Completed: { bg: '#ede7f6', color: '#4527a0' },
};

export default function AdminRequestsPage() {
    const nav = useNavigate();
    const [requests,    setRequests]    = useState([]);
    const [filter,      setFilter]      = useState('All');
    const [loading,     setLoading]     = useState(true);
    const [error,       setError]       = useState(null);
    const [editing,     setEditing]     = useState(null);
    const [editForm,    setEditForm]    = useState({});
    const [editErr,     setEditErr]     = useState(null);
    const [editLoading, setEditLoading] = useState(false);

    const load = (status) => {
        setLoading(true); setError(null);
        reliefRequestsApi.getAll(status === 'All' ? null : status)
            .then(r => setRequests(r.data))
            .catch(() => setError('Failed to load requests.'))
            .finally(() => setLoading(false));
    };

    const openEdit = (r) => {
        setEditing(r);
        setEditForm({
            area:        r.area,
            description: r.description ?? '',
            urgency:     r.urgency
        });
        setEditErr(null);
    };

    const saveEdit = async () => {
        setEditLoading(true); setEditErr(null);
        try {
            await reliefRequestsApi.update(editing.requestId, {
                area:        editForm.area        || null,
                description: editForm.description || null,
                urgency:     editForm.urgency     || null,
            });
            setEditing(null);
            load(filter);
        } catch (err) {
            setEditErr(err.response?.data?.detail ?? 'Update failed.');
        } finally {
            setEditLoading(false);
        }
    };

    useEffect(() => load(filter), [filter]);

    return (
        <>
            <AppHeader />
            <div style={pageWrap}>
                <div style={card}>

                    {/*header*/}
                    <div style={headerRow}>
                        <div>
                            <h2 style={title}>Relief Requests</h2>
                            <p style={subtitle}>View and manage all submitted relief requests.</p>
                        </div>
                        <button style={secBtn} onClick={() => nav('/admin')}>
                            ← Back
                        </button>
                    </div>

                    <hr style={divider} />

                    {/*filter tabs*/}
                    <div style={tabRow}>
                        {STATUSES.map(s => (
                            <button key={s}
                                style={filter === s ? activeTab : tabBtn}
                                onClick={() => setFilter(s)}>
                                {s}
                            </button>
                        ))}
                    </div>

                    {/*states*/}
                    {loading && <p style={muted}>Loading...</p>}
                    {error   && <p style={errorTxt}>{error}</p>}
                    {!loading && requests.length === 0 && (
                        <div style={emptyBox}>
                            <p style={{ fontSize: 32, margin: 0 }}>📋</p>
                            <p style={{ color: '#888', margin: '8px 0 0' }}>
                                No requests found.</p>
                        </div>
                    )}

                    {/*table*/}
                    {requests.length > 0 && (
                        <div style={{ overflowX: 'auto', marginTop: 16 }}>
                            <table style={table}>
                                <thead>
                                    <tr>
                                        {['Area', 'Description', 'Urgency',
                                          'Status', 'Created', 'Actions'].map(h => (
                                            <th key={h} style={th}>{h}</th>
                                        ))}
                                    </tr>
                                </thead>
                                <tbody>
                                    {requests.map(r => (
                                        <tr key={r.requestId} style={tr}>
                                            <td style={td}><b>{r.area}</b></td>
                                            <td style={{ ...td, color: '#666', maxWidth: 200 }}>
                                                {r.description ?? '—'}
                                            </td>
                                            <td style={td}>{r.urgency}</td>
                                            <td style={td}>
                                                <span style={{ ...badge, ...COLORS[r.status] }}>
                                                    {r.status}
                                                </span>
                                            </td>
                                            <td style={{ ...td, color: '#999', fontSize: 12 }}>
                                                {new Date(r.createdAt).toLocaleDateString()}
                                            </td>
                                            {/* ← Added Actions column */}
                                            <td style={td}>
                                                {r.status !== 'Completed' && (
                                                    <button style={editBtn}
                                                        onClick={() => openEdit(r)}>
                                                        Edit
                                                    </button>
                                                )}
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}

                </div>
            </div>

            {/* edit model */}
            {editing && (
                <div style={overlay}>
                    <div style={modal}>
                        <h3 style={{ marginTop: 0 }}>Edit Request</h3>

                        <label style={lbl}>Area</label>
                        <input
                            value={editForm.area}
                            onChange={e => setEditForm(p => ({ ...p, area: e.target.value }))}
                            style={inp}
                        />

                        <label style={lbl}>Description</label>
                        <textarea
                            value={editForm.description}
                            rows={3}
                            onChange={e => setEditForm(p => ({ ...p, description: e.target.value }))}
                            style={{ ...inp, resize: 'vertical' }}
                        />

                        <label style={lbl}>Urgency</label>
                        <select
                            value={editForm.urgency}
                            onChange={e => setEditForm(p => ({ ...p, urgency: e.target.value }))}
                            style={inp}>
                            {URGENCY.map(u => <option key={u}>{u}</option>)}
                        </select>

                        {editErr && (
                            <p style={{ color: '#c0392b', fontSize: 13 }}>{editErr}</p>
                        )}

                        <div style={{ display: 'flex', gap: 8, marginTop: 16 }}>
                            <button style={primBtn} onClick={saveEdit}
                                disabled={editLoading}>
                                {editLoading ? 'Saving...' : 'Save'}
                            </button>
                            <button style={secBtn} onClick={() => setEditing(null)}
                                disabled={editLoading}>
                                Cancel
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}

//styles
const pageWrap  = { minHeight: '100vh', background: '#f5f6fa',
                    display: 'flex', justifyContent: 'center',
                    padding: '48px 16px',
                    fontFamily: "'Segoe UI', Inter, sans-serif" };
const card      = { background: '#fff', borderRadius: 16,
                    boxShadow: '0 4px 24px rgba(0,0,0,0.08)',
                    padding: '36px 40px', width: '100%', maxWidth: 900 };
const headerRow = { display: 'flex', justifyContent: 'space-between',
                    alignItems: 'flex-start', gap: 16 };
const title     = { margin: 0, fontSize: 22, fontWeight: 700,
                    color: '#1a1a2e', letterSpacing: '-0.3px' };
const subtitle  = { margin: '4px 0 0', fontSize: 13, color: '#888' };
const divider   = { border: 'none', borderTop: '1px solid #f0f0f0',
                    margin: '20px 0 24px' };
const tabRow    = { display: 'flex', gap: 8, flexWrap: 'wrap',
                    marginBottom: 20 };
const tabBtn    = { padding: '7px 18px', borderRadius: 20,
                    border: '1.5px solid #e8e8e8', background: '#fff',
                    cursor: 'pointer', fontSize: 13, color: '#555',
                    fontFamily: "'Segoe UI', Inter, sans-serif" };
const activeTab = { ...tabBtn, background: '#1a1a2e', color: '#fff',
                    border: '1.5px solid #1a1a2e' };
const muted     = { color: '#888', textAlign: 'center', padding: '32px 0' };
const errorTxt  = { color: '#c0392b', fontSize: 13 };
const emptyBox  = { textAlign: 'center', padding: '40px 0' };
const table     = { width: '100%', borderCollapse: 'collapse', fontSize: 13 };
const th        = { textAlign: 'left', padding: '10px 14px',
                    background: '#f9f9f9', fontWeight: 700,
                    borderBottom: '2px solid #f0f0f0', color: '#444' };
const tr        = { borderBottom: '1px solid #f5f5f5' };
const td        = { padding: '12px 14px', verticalAlign: 'top' };
const badge     = { fontSize: 11, fontWeight: 700, padding: '4px 12px',
                    borderRadius: 20, whiteSpace: 'nowrap' };

// ← New styles added below
const editBtn   = { padding: '5px 14px', borderRadius: 8,
                    border: '1px solid #ddd', background: '#fff',
                    cursor: 'pointer', fontSize: 12 };
const overlay   = { position: 'fixed', inset: 0,
                    background: 'rgba(0,0,0,0.4)',
                    display: 'flex', alignItems: 'center',
                    justifyContent: 'center', zIndex: 1000 };
const modal     = { background: '#fff', borderRadius: 16,
                    padding: 28, width: '100%', maxWidth: 460,
                    boxShadow: '0 8px 40px rgba(0,0,0,0.18)' };
const lbl       = { display: 'block', fontSize: 13,
                    fontWeight: 600, marginBottom: 6, marginTop: 14 };
const inp       = { width: '100%', padding: '10px 12px',
                    borderRadius: 10, border: '1px solid #ddd',
                    fontSize: 14, boxSizing: 'border-box',
                    fontFamily: "'Segoe UI', Inter, sans-serif" };
const primBtn   = { flex: 1, padding: 11, borderRadius: 10,
                    border: 'none', background: '#1a1a2e',
                    color: '#fff', cursor: 'pointer', fontSize: 14,
                    fontFamily: "'Segoe UI', Inter, sans-serif" };
const secBtn    = { padding: '10px 18px', borderRadius: 10,
                    border: '1.5px solid #e8e8e8', background: '#fff',
                    color: '#555', cursor: 'pointer', fontSize: 13,
                    fontFamily: "'Segoe UI', Inter, sans-serif" };