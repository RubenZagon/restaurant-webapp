interface TableIconProps {
  tableNumber: number
  isOccupied?: boolean
  onClick: () => void
}

export const TableIcon = ({ tableNumber, isOccupied = false, onClick }: TableIconProps) => {
  return (
    <div
      onClick={onClick}
      style={{
        cursor: 'pointer',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        gap: '8px',
        transition: 'transform 0.2s',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = 'scale(1.1)'
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = 'scale(1)'
      }}
    >
      <svg
        width="120"
        height="120"
        viewBox="0 0 120 120"
        xmlns="http://www.w3.org/2000/svg"
      >
        {/* Table top - rounded rectangle */}
        <rect
          x="20"
          y="30"
          width="80"
          height="60"
          rx="8"
          fill={isOccupied ? '#ffcccc' : '#8B4513'}
          stroke={isOccupied ? '#ff6666' : '#654321'}
          strokeWidth="2"
        />

        {/* Wood grain effect */}
        <line x1="25" y1="45" x2="95" y2="45" stroke="#654321" strokeWidth="1" opacity="0.3" />
        <line x1="25" y1="60" x2="95" y2="60" stroke="#654321" strokeWidth="1" opacity="0.3" />
        <line x1="25" y1="75" x2="95" y2="75" stroke="#654321" strokeWidth="1" opacity="0.3" />

        {/* Table number in the center */}
        <text
          x="60"
          y="65"
          textAnchor="middle"
          fontSize="28"
          fontWeight="bold"
          fill={isOccupied ? '#cc0000' : '#FFF8DC'}
          fontFamily="Arial, sans-serif"
        >
          {tableNumber}
        </text>

        {/* Table legs */}
        <rect x="25" y="90" width="8" height="20" fill="#654321" rx="2" />
        <rect x="87" y="90" width="8" height="20" fill="#654321" rx="2" />
      </svg>

      <div
        style={{
          fontSize: '14px',
          fontWeight: 'bold',
          color: isOccupied ? '#cc0000' : '#333',
        }}
      >
        Mesa {tableNumber}
        {isOccupied && <div style={{ fontSize: '12px', color: '#cc0000' }}>Ocupada</div>}
      </div>
    </div>
  )
}
