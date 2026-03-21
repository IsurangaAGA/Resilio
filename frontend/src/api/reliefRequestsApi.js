import { http } from './httpClient';

export const reliefRequestsApi = {
    create: (data) => http.post('/api/relief-requests', data),

    getAll: (status) =>
        http.get('/api/relief-requests', {
            params: status ? { status } : {},
        }),

    update:  (id, data) => http.put(`/api/relief-requests/${id}`, data),
    
    delete:  (id)       => http.delete(`/api/relief-requests/${id}`),

    getMyRequests: () => http.get('/api/relief-requests/mine'),
    
};