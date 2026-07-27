/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      colors: {
        base: '#F8FAFC',
        surface: '#FFFFFF',
        elevated: '#F1F5F9',
        accent: {
          primary: '#4F46E5',
          glow: '#6366F1',
          green: '#059669',
          amber: '#D97706',
        },
        primary: '#0F172A',
        muted: '#64748B',
        border: '#E2E8F0',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        display: ['"DM Sans"', 'Inter', 'sans-serif'],
        mono: ['"JetBrains Mono"', 'monospace'],
      },
      boxShadow: {
        glow: '0 0 60px -12px rgba(79, 70, 229, 0.30)',
        'glow-sm': '0 0 30px -8px rgba(79, 70, 229, 0.20)',
        card: '0 4px 24px rgba(15, 23, 42, 0.06)',
      },
      animation: {
        float: 'float 3s ease-in-out infinite',
      },
      keyframes: {
        float: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-8px)' },
        },
      },
      backgroundImage: {
        'hero-gradient': 'linear-gradient(135deg, #4F46E5 0%, #7C3AED 100%)',
        'cta-gradient': 'linear-gradient(135deg, #EEF2FF 0%, #FAF5FF 50%, #F8FAFC 100%)',
        'halo': 'radial-gradient(ellipse at center, rgba(79,70,229,0.10) 0%, transparent 70%)',
      },
    },
  },
  plugins: [],
}
