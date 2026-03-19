// src/pages/victim/NewRequestPage.jsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AppHeader from '../../components/AppHeader/AppHeader';
import { reliefRequestsApi } from '../../api/reliefRequestsApi';

const URGENCY = ['Low', 'Medium', 'High', 'Critical'];

export default function NewRequestPage() {
    const nav = useNavigate();
    const [form, setForm] = useState({ area: '', description: '', urgency: 'Medium' });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const onChange = e => setForm(p => ({ ...p, [e.target.name]: e.target.value }));

    const onSubmit = async () => {
        setError(null);
        if (!form.area.trim()) return setError('Area is required.');
        setLoading(true);
        try {
            await reliefRequestsApi.create({
                area:        form.area.trim(),
                description: form.description.trim() || null,
                urgency:     form.urgency,
            });
            nav('/victim');
        } catch (err) {
            setError(err.response?.data?.detail ?? 'Failed to submit.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <AppHeader />
            <div style={pageWrap}>
                <div style={card}>

                    {/*header*/}
                    <div style={cardHeader}>
                        <span style={iconCircle}>🆘</span>
                        <div>
                            <h2 style={title}>Request Help</h2>
                            <p style={subtitle}>Fill in the details below and our team will respond promptly.</p>
                        </div>
                    </div>

                    <hr style={divider} />

                    {/*area*/}
                    <div style={fieldWrap}>
                        <label style={lbl}>Area / Location <span style={req}>*</span></label>
                        <input
                            name='area'
                            value={form.area}
                            onChange={onChange}
                            style={inp}
                            placeholder='e.g. Colombo 07, Western Province'
                        />
                    </div>

                    {/*description*/}
                    <div style={fieldWrap}>
                        <label style={lbl}>Description <span style={optional}>(optional)</span></label>
                        <textarea
                            name='description'
                            value={form.description}
                            onChange={onChange}
                            rows={4}
                            style={{ ...inp, resize: 'vertical', lineHeight: '1.6' }}
                            placeholder='Describe what kind of help is needed...'
                        />
                    </div>

                    {/*urgency*/}
                    <div style={fieldWrap}>
                        <label style={lbl}>Urgency Level <span style={req}>*</span></label>
                        <select name='urgency' value={form.urgency} onChange={onChange} style={inp}>
                            {URGENCY.map(u => <option key={u}>{u}</option>)}
                        </select>
                        <p style={hint}>Select how urgently help is needed.</p>
                    </div>

                    {/*error*/}
                    {error && (
                        <div style={errorBox}>
                            {error}
                        </div>
                    )}

                    {/*buttons*/}
                    <div style={btnRow}>
                        <button style={secBtn} onClick={() => nav('/victim')} disabled={loading}>
                            Cancel
                        </button>
                        <button style={primBtn} onClick={onSubmit} disabled={loading}>
                            {loading ? 'Submitting...' : 'Submit Request'}
                        </button>
                    </div>

                </div>
            </div>
        </>
    );
}

// layout
const pageWrap = {
    minHeight: '100vh',
    background: '#f5f6fa',
    display: 'flex',
    alignItems: 'flex-start',
    justifyContent: 'center',
    padding: '48px 16px',
    fontFamily: "'Segoe UI', Inter, sans-serif",
};

const card = {
    background: '#ffffff',
    borderRadius: 16,
    boxShadow: '0 4px 24px rgba(0,0,0,0.08)',
    padding: '36px 40px',
    width: '100%',
    maxWidth: 520,
};

// card header
const cardHeader = {
    display: 'flex',
    alignItems: 'center',
    gap: 16,
    marginBottom: 20,
};

const iconCircle = {
    fontSize: 28,
    background: '#fff3f3',
    borderRadius: '50%',
    width: 52,
    height: 52,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0,
};

const title = {
    margin: 0,
    fontSize: 22,
    fontWeight: 700,
    color: '#1a1a2e',
    letterSpacing: '-0.3px',
};

const subtitle = {
    margin: '4px 0 0',
    fontSize: 13,
    color: '#888',
    lineHeight: 1.5,
};

const divider = {
    border: 'none',
    borderTop: '1px solid #f0f0f0',
    margin: '0 0 24px',
};

// fields
const fieldWrap = {
    marginBottom: 20,
};

const lbl = {
    display: 'block',
    fontSize: 13,
    fontWeight: 600,
    color: '#333',
    marginBottom: 7,
    letterSpacing: '0.1px',
};

const req = {
    color: '#e53935',
    marginLeft: 2,
};

const optional = {
    fontWeight: 400,
    color: '#aaa',
    fontSize: 12,
    marginLeft: 4,
};

const inp = {
    width: '100%',
    padding: '11px 14px',
    borderRadius: 10,
    border: '1.5px solid #e8e8e8',
    fontSize: 14,
    color: '#1a1a2e',
    boxSizing: 'border-box',
    fontFamily: "'Segoe UI', Inter, sans-serif",
    outline: 'none',
    transition: 'border 0.2s',
    background: '#fafafa',
};

const hint = {
    margin: '6px 0 0',
    fontSize: 12,
    color: '#aaa',
};

// error msgs
const errorBox = {
    background: '#fff5f5',
    border: '1px solid #fcc',
    borderRadius: 10,
    padding: '10px 14px',
    fontSize: 13,
    color: '#c0392b',
    marginBottom: 16,
};

// button
const btnRow = {
    display: 'flex',
    gap: 10,
    marginTop: 8,
};

const primBtn = {
    flex: 1,
    padding: '13px 0',
    borderRadius: 10,
    border: 'none',
    background: '#1a1a2e',
    color: '#fff',
    fontSize: 15,
    fontWeight: 600,
    cursor: 'pointer',
    letterSpacing: '0.2px',
    fontFamily: "'Segoe UI', Inter, sans-serif",
};

const secBtn = {
    flex: 1,
    padding: '13px 0',
    borderRadius: 10,
    border: '1.5px solid #e8e8e8',
    background: '#fff',
    color: '#555',
    fontSize: 15,
    fontWeight: 500,
    cursor: 'pointer',
    fontFamily: "'Segoe UI', Inter, sans-serif",
};