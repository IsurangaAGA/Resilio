import { http } from './httpClient';

export const reliefRequestsApi = {
    create: (data) => http.post('/api/relief-requests', data),
    // get update delete-for later
};