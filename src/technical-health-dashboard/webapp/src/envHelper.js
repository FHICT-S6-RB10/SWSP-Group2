export const envGet = key => {
    return window._env_[key] || import.meta.env[`VITE_${key}`];
}
