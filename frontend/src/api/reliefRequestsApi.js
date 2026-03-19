import { http } from './httpClient';

export const reliefRequestsApi = {
    create: (data) => http.post('/api/relief-requests', data),

    getAll: (status) =>
        http.get('/api/relief-requests', {
            params: status ? { status } : {},
        }),

    // update delete, -for later
};