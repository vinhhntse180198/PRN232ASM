/** false = không gọi OpenAlex API (chỉ thư viện local, không tốn quota) */
export const OPENALEX_ENABLED = import.meta.env.VITE_OPENALEX_ENABLED === 'true'
